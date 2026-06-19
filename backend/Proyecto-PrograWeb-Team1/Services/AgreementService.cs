using Google.Cloud.Firestore;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Models;

namespace Proyecto_PrograWeb_Team1.Services;

public class AgreementService
{
    private readonly FirebaseService _firebaseService;

    public AgreementService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<Agreement> Create(CreateAgreementDto dto, string mediatorId)
    {
        // Verificar que el caso existe
        var caseDoc = await _firebaseService.GetCollection("denuncia")
            .Document(dto.CaseId)
            .GetSnapshotAsync();

        if (!caseDoc.Exists)
            throw new Exception("Caso no encontrado");

        var caseData = caseDoc.ToDictionary();

        // Verificar que no exista ya un acuerdo formalizado para este caso
        var existingAgreements = await _firebaseService.GetCollection("agreements")
            .WhereEqualTo("CaseId", dto.CaseId)
            .GetSnapshotAsync();

        foreach (var doc in existingAgreements.Documents)
        {
            var data = doc.ToDictionary();
            if (data.ContainsKey("IsImmutable") && (bool)data["IsImmutable"])
                throw new Exception("Este caso ya tiene un acuerdo formalizado. No se puede crear otro.");
        }

        var agreement = new Agreement
        {
            Id = Guid.NewGuid().ToString(),
            CaseId = dto.CaseId,
            MediatorId = mediatorId,
            AgreementText = dto.AgreementText,
            Points = dto.Points.Select(p => new AgreementPoint
            {
                Description = p.Description,
                Deadline = p.Deadline,
                ComplianceStatus = "Pendiente"
            }).ToList(),
            ReporterId = caseData.ContainsKey("UserId") ? caseData["UserId"].ToString()! : "",
            RespondentId = caseData.ContainsKey("User-report") ? caseData["User-report"].ToString()! : "",
            Status = "borrador",
            CreatedAt = DateTime.UtcNow
        };

        // Guardar el acuerdo
        await _firebaseService.GetCollection("agreements")
            .Document(agreement.Id)
            .SetAsync(new Dictionary<string, object>
            {
                { "Id", agreement.Id },
                { "CaseId", agreement.CaseId },
                { "MediatorId", agreement.MediatorId },
                { "AgreementText", agreement.AgreementText },
                { "Points", agreement.Points.Select(p => new Dictionary<string, object>
                {
                    { "Description", p.Description },
                    { "Deadline", Timestamp.FromDateTime(
                    DateTime.SpecifyKind(
                        p.Deadline,
                        DateTimeKind.Utc))
                },
                    { "ComplianceStatus", p.ComplianceStatus },
                    { "ReportedBy", null! },
                    { "ComplianceReportedAt", null! }
                }).ToList() },
                { "ConfirmedByReporter", false },
                { "ConfirmedByRespondent", false },
                { "ReporterConfirmedAt", null! },
                { "RespondentConfirmedAt", null! },
                { "ReporterId", agreement.ReporterId },
                { "RespondentId", agreement.RespondentId },
                { "Status", "borrador" },
                { "FormalizedAt", null! },
                { "IsImmutable", false },
                { "CreatedAt", agreement.CreatedAt }
            });

        return agreement;
    }

    public async Task<Agreement> GetById(string id)
    {
        var doc = await _firebaseService.GetCollection("agreements")
            .Document(id)
            .GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Acuerdo no encontrado");

        return MapToAgreement(doc.ToDictionary());
    }

    public async Task<List<Agreement>> GetByCase(string caseId)
    {
        var snapshot = await _firebaseService.GetCollection("agreements")
            .WhereEqualTo("CaseId", caseId)
            .GetSnapshotAsync();

        var agreements = new List<Agreement>();
        foreach (var doc in snapshot.Documents)
        {
            agreements.Add(MapToAgreement(doc.ToDictionary()));
        }

        return agreements;
    }

    public async Task<Agreement> ConfirmAgreement(string agreementId, string userId)
    {
        var docRef = _firebaseService.GetCollection("agreements").Document(agreementId);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Acuerdo no encontrado");

        var data = doc.ToDictionary();

        if (data.ContainsKey("IsImmutable") && (bool)data["IsImmutable"])
            throw new Exception("El acuerdo ya está formalizado y no se puede modificar");

        var reporterId = data["ReporterId"].ToString()!;
    
    //ESTA PARTE DE ACA LO QUE HACE ES QUE SACA EL NOMBRE Y EL ID, PQ SOLO PIDE EL ID PA REPORTAR
    var respondentId = data["RespondentId"].ToString()!;

    var userDoc = await _firebaseService
    .GetCollection("users")
    .Document(userId)
    .GetSnapshotAsync();

    var fullName = userDoc
    .ToDictionary()["FullName"]
    .ToString();

    var isReporter = userId == reporterId;
        var isRespondent = fullName == respondentId;


        if (!isReporter && !isRespondent)
            throw new Exception("No eres parte de este acuerdo");

        var now = DateTime.UtcNow;
        var updates = new Dictionary<string, object>();

        if (isReporter)
        {
            updates["ConfirmedByReporter"] = true;
            updates["ReporterConfirmedAt"] = now;
        }
        else
        {
            updates["ConfirmedByRespondent"] = true;
            updates["RespondentConfirmedAt"] = now;
        }

        await docRef.UpdateAsync(updates);

        // Verificar si ambas partes ya confirmaron
        var updated = await docRef.GetSnapshotAsync();
        var updatedData = updated.ToDictionary();

        var bothConfirmed = (bool)updatedData["ConfirmedByReporter"] && (bool)updatedData["ConfirmedByRespondent"];

        if (bothConfirmed)
        {
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "Status", "formalizado" },
                { "FormalizedAt", DateTime.UtcNow },
                { "IsImmutable", true }
            });

            // Actualizar el estado del caso a "resuelto"
            var caseId = updatedData["CaseId"].ToString()!;
            var caseRef = _firebaseService.GetCollection("denuncia").Document(caseId);
            await caseRef.UpdateAsync(new Dictionary<string, object>
            {
                { "Status", "Completado" }
            });
        }
        else
        {
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "Status", "pendiente_confirmacion" }
            });
        }

        var final = await docRef.GetSnapshotAsync();
        return MapToAgreement(final.ToDictionary());
    }

    public async Task<Agreement> ReportCompliance(ReportComplianceDto dto, string userId)
    {
        var docRef = _firebaseService.GetCollection("agreements").Document(dto.AgreementId);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Acuerdo no encontrado");

        var data = doc.ToDictionary();

        if (!data.ContainsKey("IsImmutable") || !(bool)data["IsImmutable"])
            throw new Exception("El acuerdo debe estar formalizado para reportar cumplimiento");

        var reporterId = data["ReporterId"].ToString()!;
        var respondentId = data["RespondentId"].ToString()!;

        if (userId != reporterId && userId != respondentId)
            throw new Exception("No eres parte de este acuerdo");

        // Obtener los puntos actuales
        var points = data["Points"] as List<object>;
        if (points == null || dto.PointIndex >= points.Count)
            throw new Exception("Punto de acuerdo no válido");

        // Actualizar el punto específico
        var pointRef = $"Points[{dto.PointIndex}]";
        
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { $"{pointRef}.ComplianceStatus", dto.ComplianceStatus },
            { $"{pointRef}.ReportedBy", userId },
            { $"{pointRef}.ComplianceReportedAt", DateTime.UtcNow }
        });

        var updated = await docRef.GetSnapshotAsync();
        return MapToAgreement(updated.ToDictionary());
    }

    private Agreement MapToAgreement(Dictionary<string, object> data)
    {
        var points = new List<AgreementPoint>();
        if (data.ContainsKey("Points") && data["Points"] is List<object> rawPoints)
        {
            foreach (var raw in rawPoints)
            {
                if (raw is Dictionary<string, object> p)
                {
                    points.Add(new AgreementPoint
                    {
                        Description = p.ContainsKey("Description") ? p["Description"].ToString()! : "",
                        Deadline = p.ContainsKey("Deadline") && p["Deadline"] != null
                            ? ((Google.Cloud.Firestore.Timestamp)p["Deadline"]).ToDateTime()
                            : DateTime.UtcNow,
                        ComplianceStatus = p.ContainsKey("ComplianceStatus") ? p["ComplianceStatus"].ToString()! : "Pendiente",
                        ReportedBy = p.ContainsKey("ReportedBy") ? p["ReportedBy"]?.ToString() : null,
                        ComplianceReportedAt = p.ContainsKey("ComplianceReportedAt") && p["ComplianceReportedAt"] != null
                            ? ((Google.Cloud.Firestore.Timestamp)p["ComplianceReportedAt"]).ToDateTime()
                            : null
                    });
                }
            }
        }

        return new Agreement
        {
            Id = data["Id"].ToString()!,
            CaseId = data["CaseId"].ToString()!,
            MediatorId = data["MediatorId"].ToString()!,
            AgreementText = data["AgreementText"].ToString()!,
            Points = points,
            ConfirmedByReporter = data.ContainsKey("ConfirmedByReporter") && (bool)data["ConfirmedByReporter"],
            ConfirmedByRespondent = data.ContainsKey("ConfirmedByRespondent") && (bool)data["ConfirmedByRespondent"],
            ReporterConfirmedAt = data.ContainsKey("ReporterConfirmedAt") && data["ReporterConfirmedAt"] != null
                ? ((Google.Cloud.Firestore.Timestamp)data["ReporterConfirmedAt"]).ToDateTime()
                : null,
            RespondentConfirmedAt = data.ContainsKey("RespondentConfirmedAt") && data["RespondentConfirmedAt"] != null
                ? ((Google.Cloud.Firestore.Timestamp)data["RespondentConfirmedAt"]).ToDateTime()
                : null,
            ReporterId = data["ReporterId"].ToString()!,
            RespondentId = data["RespondentId"].ToString()!,
            Status = data["Status"].ToString()!,
            FormalizedAt = data.ContainsKey("FormalizedAt") && data["FormalizedAt"] != null
                ? ((Google.Cloud.Firestore.Timestamp)data["FormalizedAt"]).ToDateTime()
                : null,
            IsImmutable = data.ContainsKey("IsImmutable") && (bool)data["IsImmutable"],
            CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
        };
    }
}
