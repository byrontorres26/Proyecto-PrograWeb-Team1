using Google.Cloud.Firestore;

namespace Proyecto_PrograWeb_Team1.Services;

public class NotificationService
{
    private readonly FirebaseService _firebaseService;

    public NotificationService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<List<object>> GetMyNotifications(
        string userId,
        string role)
    {
        var notifications = new List<object>();

        var userDoc = await _firebaseService
            .GetCollection("users")
            .Document(userId)
            .GetSnapshotAsync();

        if (!userDoc.Exists)
            return notifications;

        var userData = userDoc.ToDictionary();

        var fullName = userData["FullName"].ToString();

        // Buscar denuncias donde el usuario fue reportado
        var denunciasSnapshot = await _firebaseService
            .GetCollection("denuncia")
            .WhereEqualTo("User-report", fullName)
            .GetSnapshotAsync();

        foreach (var denuncia in denunciasSnapshot.Documents)
        {
            var data = denuncia.ToDictionary();

            var status = data["Status"].ToString();

            // Ignorar casos cerrados
            if (status == "Completado" ||
                status == "Cerradosinacuerdo")
            {
                continue;
            }

            notifications.Add(new
            {
                type = "denuncia",
                title = "Has sido reportado",
                caseId = data["Id"].ToString(),
                status = status,
                category = data["Categoria"].ToString()
            });

            // Buscar sesiones asociadas al caso
            var sesionesSnapshot = await _firebaseService
                .GetCollection("sessions")
                .WhereEqualTo("CaseId", data["Id"].ToString())
                .GetSnapshotAsync();

            foreach (var sesion in sesionesSnapshot.Documents)
            {
                var sessionData = sesion.ToDictionary();

                notifications.Add(new
                {
                    type = "sesion",
                    title = "Tienes una sesión programada",
                    date = ((Timestamp)sessionData["ScheduledDate"]).ToDateTime(),
                    modality = sessionData["Modality"].ToString(),
                    meetingLink = sessionData.ContainsKey("MeetingLink")
                        ? sessionData["MeetingLink"]?.ToString()
                        : null
                });
            }
        }

        return notifications;
    }
}
