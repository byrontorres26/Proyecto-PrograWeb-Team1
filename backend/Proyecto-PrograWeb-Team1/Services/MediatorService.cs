using Google.Cloud.Firestore;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Models;

namespace Proyecto_PrograWeb_Team1.Services;

public class MediatorService
{
    private readonly FirebaseService _firebaseService;

    public MediatorService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<Mediator> Create(CreateMediatorDto dto)
    {
        // Verificar que no exista un mediador con ese email
        var collection = _firebaseService.GetCollection("mediators");
        var existing = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (existing.Count > 0)
            throw new Exception("Ya existe un mediador con ese correo");

        var mediator = new Mediator
        {
            Id = Guid.NewGuid().ToString(),
            FullName = dto.FullName,
            Email = dto.Email,
            Zone = dto.Zone,
            Specialty = dto.Specialty,
            Availability = dto.Availability,
            IsActive = true,
            ActiveCasesCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await collection.Document(mediator.Id).SetAsync(new Dictionary<string, object>
        {
            { "Id", mediator.Id },
            { "FullName", mediator.FullName },
            { "Email", mediator.Email },
            { "Zone", mediator.Zone },
            { "Specialty", mediator.Specialty },
            { "Availability", mediator.Availability },
            { "IsActive", mediator.IsActive },
            { "UserId", null! },
            { "ActiveCasesCount", mediator.ActiveCasesCount },
            { "CreatedAt", mediator.CreatedAt }
        });

        return mediator;
    }

    public async Task<List<Mediator>> GetAll(bool includeInactive = false)
    {
        var collection = _firebaseService.GetCollection("mediators");
        var snapshot = includeInactive
            ? await collection.GetSnapshotAsync()
            : await collection.WhereEqualTo("IsActive", true).GetSnapshotAsync();

        var mediators = new List<Mediator>();

        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();
            mediators.Add(MapToMediator(data));
        }

        return mediators;
    }

    public async Task<Mediator> GetById(string id)
    {
        var doc = await _firebaseService.GetCollection("mediators")
            .Document(id)
            .GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Mediador no encontrado");

        var data = doc.ToDictionary();
        return MapToMediator(data);
    }

    public async Task<Mediator> Update(string id, UpdateMediatorDto dto)
    {
        var docRef = _firebaseService.GetCollection("mediators").Document(id);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Mediador no encontrado");

        var updates = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(dto.FullName))
            updates["FullName"] = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Zone))
            updates["Zone"] = dto.Zone;
        if (!string.IsNullOrEmpty(dto.Specialty))
            updates["Specialty"] = dto.Specialty;
        if (!string.IsNullOrEmpty(dto.Availability))
            updates["Availability"] = dto.Availability;

        if (updates.Count > 0)
            await docRef.UpdateAsync(updates);

        // Devolver el documento actualizado
        var updated = await docRef.GetSnapshotAsync();
        return MapToMediator(updated.ToDictionary());
    }

    public async Task<Mediator> Deactivate(string id)
    {
        var docRef = _firebaseService.GetCollection("mediators").Document(id);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Mediador no encontrado");

        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "IsActive", false }
        });

        var updated = await docRef.GetSnapshotAsync();
        return MapToMediator(updated.ToDictionary());
    }

    public async Task<Mediator> Activate(string id)
    {
        var docRef = _firebaseService.GetCollection("mediators").Document(id);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists)
            throw new Exception("Mediador no encontrado");

        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "IsActive", true }
        });

        var updated = await docRef.GetSnapshotAsync();
        return MapToMediator(updated.ToDictionary());
    }

    private Mediator MapToMediator(Dictionary<string, object> data)
    {
        return new Mediator
        {
            Id = data["Id"].ToString()!,
            FullName = data["FullName"].ToString()!,
            Email = data["Email"].ToString()!,
            Zone = data["Zone"].ToString()!,
            Specialty = data["Specialty"].ToString()!,
            Availability = data.ContainsKey("Availability") ? data["Availability"].ToString()! : string.Empty,
            IsActive = data.ContainsKey("IsActive") ? (bool)data["IsActive"] : true,
            UserId = data.ContainsKey("UserId") ? data["UserId"]?.ToString() : null,
            ActiveCasesCount = data.ContainsKey("ActiveCasesCount") ? Convert.ToInt32(data["ActiveCasesCount"]) : 0,
            CreatedAt = data.ContainsKey("CreatedAt")
                ? ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
                : DateTime.UtcNow
        };
    }
}
