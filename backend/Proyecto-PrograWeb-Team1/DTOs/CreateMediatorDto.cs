namespace Proyecto_PrograWeb_Team1.DTOs;

public class CreateMediatorDto
{
    // Lo que el admin manda cuando registra un mediador
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Availability { get; set; } = string.Empty;
}
