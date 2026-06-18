using Google.Cloud.Firestore;

namespace Proyecto_PrograWeb_Team1.Services;

public class ReportService
{
    private readonly FirebaseService _firebaseService;

    public ReportService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<object> GetDashboard()
    {
        // Contar ciudadanos (colección "users")
        var usersSnapshot = await _firebaseService.GetCollection("users").GetSnapshotAsync();
        var totalCitizens = usersSnapshot.Documents.Count;

        // Contar mediadores (colección "mediators")
        var mediatorsSnapshot = await _firebaseService.GetCollection("mediators")
            .WhereEqualTo("IsActive", true)
            .GetSnapshotAsync();
        var totalMediators = mediatorsSnapshot.Documents.Count;

        // Contar casos (colección "denuncia")
        var casesSnapshot = await _firebaseService.GetCollection("denuncia").GetSnapshotAsync();
        var totalCases = casesSnapshot.Documents.Count;

        // Casos por estado
        var casesByStatus = new Dictionary<string, int>();
        foreach (var doc in casesSnapshot.Documents)
        {
            var data = doc.ToDictionary();
            var status = data.ContainsKey("Status") ? data["Status"].ToString()! : "Desconocido";
            
            if (casesByStatus.ContainsKey(status))
                casesByStatus[status]++;
            else
                casesByStatus[status] = 1;
        }

        return new
        {
            totalCitizens,
            totalMediators,
            totalCases,
            casesByStatus
        };
    }
}
