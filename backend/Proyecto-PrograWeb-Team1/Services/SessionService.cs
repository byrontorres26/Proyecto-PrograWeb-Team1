using Google.Cloud.Firestore;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Models;

namespace Proyecto_PrograWeb_Team1.Services;

public class SessionService
{
    private readonly FirebaseService _firebaseService;

    public SessionService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<MediationSession> Create(CreateSessionDto dto, string mediatorId)
    {
        var session = new MediationSession
        {
            Id = Guid.NewGuid().ToString(),
            CaseId = dto.CaseId,
            MediatorId = mediatorId,
            ScheduledDate = DateTime.Parse(dto.ScheduledDate).ToUniversalTime(),
            Modality = dto.Modality,
            MeetingLink = dto.Modality == "virtual" ? dto.MeetingLink : null,
            Status = "programada",
            CreatedAt = DateTime.UtcNow
        };

        await _firebaseService.GetCollection("sessions")
            .Document(session.Id)
            .SetAsync(new Dictionary<string, object>
            {
                { "Id", session.Id },
                { "CaseId", session.CaseId },
                { "MediatorId", session.MediatorId },
                { "ScheduledDate", session.ScheduledDate },
                { "Modality", session.Modality },
                { "MeetingLink", session.MeetingLink ?? (object)null! },
                { "Status", session.Status },
                { "SessionNotes", null! },
                { "ReporterConfirmed", session.ReporterConfirmed },
                { "RespondentConfirmed", session.RespondentConfirmed },
                { "CreatedAt", session.CreatedAt },
                { "CompletedAt", null! }
            });

        return session;
    }

    public async Task<List<MediationSession>> GetByCase(string caseId)
    {
        var snapshot = await _firebaseService.GetCollection("sessions")
            .WhereEqualTo("CaseId", caseId)
            .GetSnapshotAsync();

        var sessions = new List<MediationSession>();
        foreach (var doc in snapshot.Documents)
        {
            sessions.Add(MapToSession(doc.ToDictionary()));
        }

        return sessions;
    }

    public async Task<MediationSession> GetById(string id)
    {
        var doc = await _firebaseService.GetCollection("sessions")
            .Document(id)
            .GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Sesión no encontrada");

        return MapToSession(doc.ToDictionary());
    }

    public async Task<MediationSession> CompleteSession(string id, CompleteSessionDto dto)
    {
        var docRef = _firebaseService.GetCollection("sessions").Document(id);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Sesión no encontrada");

        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "Status", "realizada" },
            { "SessionNotes", dto.SessionNotes },
            { "CompletedAt", DateTime.UtcNow }
        });

        var updated = await docRef.GetSnapshotAsync();
        return MapToSession(updated.ToDictionary());
    }

    public async Task<MediationSession> ConfirmAttendance(string id, string userId, bool isReporter)
    {
        var docRef = _firebaseService.GetCollection("sessions").Document(id);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Sesión no encontrada");

        var field = isReporter ? "ReporterConfirmed" : "RespondentConfirmed";
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { field, true }
        });

        var updated = await docRef.GetSnapshotAsync();
        return MapToSession(updated.ToDictionary());
    }

    public async Task<List<MediationSession>> GetByMediator(string mediatorId)
    {
        var snapshot = await _firebaseService.GetCollection("sessions")
            .WhereEqualTo("MediatorId", mediatorId)
            .GetSnapshotAsync();

        var sessions = new List<MediationSession>();
        foreach (var doc in snapshot.Documents)
        {
            sessions.Add(MapToSession(doc.ToDictionary()));
        }

        return sessions;
    }

    private MediationSession MapToSession(Dictionary<string, object> data)
    {
        return new MediationSession
        {
            Id = data["Id"].ToString()!,
            CaseId = data["CaseId"].ToString()!,
            MediatorId = data["MediatorId"].ToString()!,
            ScheduledDate = ((Google.Cloud.Firestore.Timestamp)data["ScheduledDate"]).ToDateTime(),
            Modality = data["Modality"].ToString()!,
            MeetingLink = data.ContainsKey("MeetingLink") ? data["MeetingLink"]?.ToString() : null,
            Status = data["Status"].ToString()!,
            SessionNotes = data.ContainsKey("SessionNotes") ? data["SessionNotes"]?.ToString() : null,
            ReporterConfirmed = data.ContainsKey("ReporterConfirmed") && (bool)data["ReporterConfirmed"],
            RespondentConfirmed = data.ContainsKey("RespondentConfirmed") && (bool)data["RespondentConfirmed"],
            CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime(),
            CompletedAt = data.ContainsKey("CompletedAt") && data["CompletedAt"] != null
                ? ((Google.Cloud.Firestore.Timestamp)data["CompletedAt"]).ToDateTime()
                : null
        };
    }
}
