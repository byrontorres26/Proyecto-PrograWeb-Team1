namespace Proyecto_PrograWeb_Team1.Models;

public class Mediator
{
    // Representa un mediador comunitario registrado por el admin
    public string Id { get; set; } = string.Empty;
    
    // Nombre completo del mediador
    public string FullName { get; set; } = string.Empty;
    
    // Email del mediador (para notificaciones)
    public string Email { get; set; } = string.Empty;
    
    // Zona de cobertura (ej: "Norte", "Centro", "Sur")
    public string Zone { get; set; } = string.Empty;
    
    // Especialidad (ej: "Ruido", "Límites", "General")
    public string Specialty { get; set; } = string.Empty;
    
    // Disponibilidad (ej: "Lunes-Viernes 9am-5pm")
    public string Availability { get; set; } = string.Empty;
    
    // Si está activo o fue desactivado por el admin
    public bool IsActive { get; set; } = true;
    
    // ID del usuario en el sistema (si el mediador también tiene cuenta)
    public string? UserId { get; set; }
    
    // Cuántos casos activos tiene asignados actualmente
    public int ActiveCasesCount { get; set; } = 0;
    
    // Cuándo se registró
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
