namespace Proyecto_PrograWeb_Team1.Models;

public class MediationSession
{
    // Representa una sesión de mediación programada para un caso
    public string Id { get; set; } = string.Empty;
    
    // Caso al que pertenece esta sesión
    public string CaseId { get; set; } = string.Empty;
    
    // Mediador que dirige la sesión
    public string MediatorId { get; set; } = string.Empty;
    
    // Fecha y hora programada
    public DateTime ScheduledDate { get; set; }
    
    // Modalidad: presencial / virtual
    public string Modality { get; set; } = "presencial";
    
    // Enlace para modalidad virtual
    public string? MeetingLink { get; set; }
    
    // Estado: programada / realizada / reprogramada / cancelada
    public string Status { get; set; } = "programada";
    
    // Notas de la sesión (las escribe el mediador después de realizada)
    public string? SessionNotes { get; set; }
    
    // Confirmación de las partes
    public bool ReporterConfirmed { get; set; } = false;
    public bool RespondentConfirmed { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
