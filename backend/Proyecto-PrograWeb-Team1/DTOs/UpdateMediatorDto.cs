namespace Proyecto_PrograWeb_Team1.DTOs;

public class UpdateMediatorDto
{
    // Lo que el admin puede editar de un mediador
    public string? FullName { get; set; }
    public string? Zone { get; set; }
    public string? Specialty { get; set; }
    public string? Availability { get; set; }
}
