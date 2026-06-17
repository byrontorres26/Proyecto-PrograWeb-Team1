namespace Proyecto_PrograWeb_Team1.DTOs;

public class CreateSessionDto
{
    // Lo que el mediador manda para programar una sesión
    public string CaseId { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string Modality { get; set; } = "presencial";
    public string? MeetingLink { get; set; }
}
