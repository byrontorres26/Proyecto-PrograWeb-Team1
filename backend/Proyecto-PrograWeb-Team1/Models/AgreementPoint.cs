namespace Proyecto_PrograWeb_Team1.Models;

public class AgreementPoint
{
    // Un punto específico del acuerdo (ej: "El vecino A se compromete a no poner música después de las 10pm")
    public string Description { get; set; } = string.Empty;
    
    // Fecha límite para cumplir este punto
    public DateTime Deadline { get; set; }
    
    // Estado de cumplimiento: Pendiente, Cumplido, Incumplido
    public string ComplianceStatus { get; set; } = "Pendiente";
    
    // Quién reportó el cumplimiento/incumplimiento
    public string? ReportedBy { get; set; }
    
    // Fecha en que se reportó cumplimiento/incumplimiento
    public DateTime? ComplianceReportedAt { get; set; }
}
