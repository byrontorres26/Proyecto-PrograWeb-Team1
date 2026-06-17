using Proyecto_PrograWeb_Team1.Models;

namespace Proyecto_PrograWeb_Team1.DTOs;

public class CreateAgreementDto
{
    // Lo que el mediador manda para redactar un acuerdo
    public string CaseId { get; set; } = string.Empty;
    public string AgreementText { get; set; } = string.Empty;
    public List<AgreementPointDto> Points { get; set; } = new();
}

public class AgreementPointDto
{
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
}
