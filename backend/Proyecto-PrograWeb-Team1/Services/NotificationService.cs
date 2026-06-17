using Google.Cloud.Firestore;
using Proyecto_PrograWeb_Team1.Models;

namespace Proyecto_PrograWeb_Team1.Services;

public class NotificationService
{
    private readonly FirebaseService _firebaseService;

    public NotificationService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<List<object>> GetMyNotifications(string userId, string userRole)
    {
        var notifications = new List<object>();
        var casos = new List<(Dictionary<string, object> Data, string Id)>();

        // 1. Si es ciudadano: buscar casos donde es el reportero
        if (userRole == "user" || userRole == "admin")
        {
            var reporterCases = await _firebaseService
                .GetCollection("denuncia")
                .WhereEqualTo("UserId", userId)
                .GetSnapshotAsync();

            foreach (var doc in reporterCases.Documents)
            {
                casos.Add((doc.ToDictionary(), doc.Id));
            }
        }

        // 2. Si es mediador: buscar casos donde está asignado como mediador
        if (userRole == "mediador" || userRole == "admin")
        {
            var mediatorCases = await _firebaseService
                .GetCollection("denuncia")
                .WhereEqualTo("MediatorId", userId)
                .GetSnapshotAsync();

            foreach (var doc in mediatorCases.Documents)
            {
                // Evitar duplicados si el mediador también es el reportero
                if (!casos.Any(c => c.Id == doc.Id))
                {
                    casos.Add((doc.ToDictionary(), doc.Id));
                }
            }
        }

        // 3. Para cada caso, generar notificaciones relevantes
        foreach (var (data, caseId) in casos)
        {
            var title = data.ContainsKey("Title") ? data["Title"].ToString()! : "Sin título";
            var status = data.ContainsKey("Status") ? data["Status"].ToString()! : "";
            var mediatorId = data.ContainsKey("MediatorId") ? data["MediatorId"]?.ToString() : null;

            // Notificación por estado del caso
            switch (status)
            {
                case "Nuevo":
                    notifications.Add(
                        new
                        {
                            type = "case_created",
                            message = $"Tu caso \"{title}\" fue registrado. Esperando asignación de mediador.",
                            caseId,
                            caseTitle = title,
                            timestamp = DateTime.UtcNow,
                        }
                    );
                    break;

                case "Mediacion":
                    var mediatorName = data.ContainsKey("MediatorName")
                        ? data["MediatorName"]?.ToString()
                        : null;
                    if (!string.IsNullOrEmpty(mediatorName))
                    {
                        notifications.Add(
                            new
                            {
                                type = "case_assigned",
                                message = $"Tu caso \"{title}\" fue asignado al mediador {mediatorName}.",
                                caseId,
                                caseTitle = title,
                                mediatorName,
                                timestamp = DateTime.UtcNow,
                            }
                        );
                    }
                    break;

                case "Completado":
                    notifications.Add(
                        new
                        {
                            type = "case_completed",
                            message = $"Tu caso \"{title}\" fue resuelto. Revisá el acuerdo formalizado.",
                            caseId,
                            caseTitle = title,
                            timestamp = DateTime.UtcNow,
                        }
                    );
                    break;

                case "Cerradosinacuerdo":
                    notifications.Add(
                        new
                        {
                            type = "case_closed",
                            message = $"Tu caso \"{title}\" fue cerrado sin acuerdo.",
                            caseId,
                            caseTitle = title,
                            timestamp = DateTime.UtcNow,
                        }
                    );
                    break;
            }

            // Buscar sesiones pendientes para este caso
            var sessionsSnapshot = await _firebaseService
                .GetCollection("sessions")
                .WhereEqualTo("CaseId", caseId)
                .WhereEqualTo("Status", "programada")
                .GetSnapshotAsync();

            foreach (var sessionDoc in sessionsSnapshot.Documents)
            {
                var sessionData = sessionDoc.ToDictionary();
                var scheduledDate = sessionData.ContainsKey("ScheduledDate")
                    ? ((Google.Cloud.Firestore.Timestamp)sessionData["ScheduledDate"]).ToDateTime()
                    : DateTime.UtcNow;

                var modality = sessionData.ContainsKey("Modality")
                    ? sessionData["Modality"].ToString()!
                    : "presencial";

                notifications.Add(
                    new
                    {
                        type = "session_scheduled",
                        message = $"Tenés una sesión de mediación programada para \"{title}\" el {scheduledDate:dd/MM/yyyy} a las {scheduledDate:HH:mm} ({modality}).",
                        caseId,
                        caseTitle = title,
                        sessionId = sessionDoc.Id,
                        scheduledDate,
                        modality,
                        timestamp = DateTime.UtcNow,
                    }
                );
            }

            // Buscar acuerdos pendientes de confirmación
            var agreementsSnapshot = await _firebaseService
                .GetCollection("agreements")
                .WhereEqualTo("CaseId", caseId)
                .GetSnapshotAsync();

            foreach (var agreementDoc in agreementsSnapshot.Documents)
            {
                var agreementData = agreementDoc.ToDictionary();
                var agreementStatus = agreementData.ContainsKey("Status")
                    ? agreementData["Status"].ToString()!
                    : "";
                var isImmutable =
                    agreementData.ContainsKey("IsImmutable") && (bool)agreementData["IsImmutable"];

                if (agreementStatus == "borrador" || agreementStatus == "pendiente_confirmacion")
                {
                    var confirmedByReporter =
                        agreementData.ContainsKey("ConfirmedByReporter")
                        && (bool)agreementData["ConfirmedByReporter"];
                    var confirmedByRespondent =
                        agreementData.ContainsKey("ConfirmedByRespondent")
                        && (bool)agreementData["ConfirmedByRespondent"];

                    string confirmMsg;
                    if (!confirmedByReporter && !confirmedByRespondent)
                        confirmMsg =
                            "El mediador redactó un acuerdo. Ambas partes deben confirmarlo.";
                    else if (confirmedByReporter && !confirmedByRespondent)
                        confirmMsg =
                            $"El acuerdo para \"{title}\" fue confirmado por una parte. Falta la confirmación de la otra.";
                    else if (!confirmedByReporter && confirmedByRespondent)
                        confirmMsg =
                            $"El acuerdo para \"{title}\" fue confirmado por una parte. Falta la confirmación de la otra.";
                    else
                        confirmMsg =
                            $"El acuerdo para \"{title}\" está pendiente de formalización.";

                    notifications.Add(
                        new
                        {
                            type = "agreement_pending",
                            message = confirmMsg,
                            caseId,
                            caseTitle = title,
                            agreementId = agreementDoc.Id,
                            timestamp = DateTime.UtcNow,
                        }
                    );
                }

                // Si está formalizado, revisar puntos pendientes de cumplimiento
                if (isImmutable && agreementData.ContainsKey("Points"))
                {
                    var points = agreementData["Points"] as List<object>;
                    if (points != null)
                    {
                        foreach (var rawPoint in points)
                        {
                            if (rawPoint is Dictionary<string, object> point)
                            {
                                var complianceStatus = point.ContainsKey("ComplianceStatus")
                                    ? point["ComplianceStatus"].ToString()!
                                    : "Pendiente";
                                if (complianceStatus == "Pendiente")
                                {
                                    var desc = point.ContainsKey("Description")
                                        ? point["Description"].ToString()!
                                        : "";
                                    notifications.Add(
                                        new
                                        {
                                            type = "compliance_pending",
                                            message = $"Tenés un punto del acuerdo pendiente de reportar: \"{desc}\"",
                                            caseId,
                                            caseTitle = title,
                                            agreementId = agreementDoc.Id,
                                            pointDescription = desc,
                                            timestamp = DateTime.UtcNow,
                                        }
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }

        // Ordenar por timestamp descendente (más reciente primero)
        return notifications
            .OrderByDescending(n =>
            {
                var prop = n.GetType().GetProperty("timestamp");
                return prop?.GetValue(n) is DateTime dt ? dt : DateTime.MinValue;
            })
            .ToList();
    }
}
