namespace Proyecto_PrograWeb_Team1.DTOs;

public class ReportComplianceDto
{
    // Lo que el ciudadano manda para reportar cumplimiento/incumplimiento
    public string AgreementId { get; set; } = string.Empty;
    public int PointIndex { get; set; }
    public string ComplianceStatus { get; set; } = string.Empty; // "Cumplido" o "Incumplido"
}
