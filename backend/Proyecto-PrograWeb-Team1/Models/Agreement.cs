namespace Proyecto_PrograWeb_Team1.Models;

public class Agreement
{
    // Representa el acuerdo alcanzado entre las partes en una mediación
    public string Id { get; set; } = string.Empty;
    
    // Caso al que pertenece este acuerdo
    public string CaseId { get; set; } = string.Empty;
    
    // Mediador que redactó el acuerdo
    public string MediatorId { get; set; } = string.Empty;
    
    // Texto estructurado del acuerdo
    public string AgreementText { get; set; } = string.Empty;
    
    // Puntos específicos del acuerdo con plazos
    public List<AgreementPoint> Points { get; set; } = new();
    
    // Confirmaciones de ambas partes
    public bool ConfirmedByReporter { get; set; } = false;
    public bool ConfirmedByRespondent { get; set; } = false;
    public DateTime? ReporterConfirmedAt { get; set; }
    public DateTime? RespondentConfirmedAt { get; set; }
    
    // IDs de las partes
    public string ReporterId { get; set; } = string.Empty;
    public string RespondentId { get; set; } = string.Empty;
    
    // Estado: borrador, pendiente_confirmacion, formalizado
    public string Status { get; set; } = "borrador";
    
    // Fecha de formalización (cuando ambas partes confirmaron)
    public DateTime? FormalizedAt { get; set; }
    
    // Marca de inmutable (una vez formalizado, no se puede modificar)
    public bool IsImmutable { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
