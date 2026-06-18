using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Services;

namespace Proyecto_PrograWeb_Team1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Se requiere token válido
public class MediatorController : ControllerBase
{
    private readonly MediatorService _mediatorService;

    public MediatorController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    // POST /api/mediator — Solo admin puede registrar mediadores
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateMediatorDto dto)
    {
        try
        {
            var mediator = await _mediatorService.Create(dto);
            return Ok(new
            {
                mediator.Id,
                mediator.FullName,
                mediator.Email,
                mediator.Zone,
                mediator.Specialty,
                mediator.Availability,
                mediator.IsActive
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/mediator — Admin ve todos los mediadores activos
    // GET /api/mediator?includeInactive=true — Admin ve todos incluyendo inactivos
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        try
        {
            var mediators = await _mediatorService.GetAll(includeInactive);
            return Ok(mediators);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/mediator/{id} — Admin o el propio mediador
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var mediator = await _mediatorService.GetById(id);
            return Ok(mediator);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/mediator/{id} — Solo admin edita mediador
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateMediatorDto dto)
    {
        try
        {
            var mediator = await _mediatorService.Update(id, dto);
            return Ok(mediator);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH /api/mediator/{id}/deactivate — Solo admin desactiva
    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Deactivate(string id)
    {
        try
        {
            var mediator = await _mediatorService.Deactivate(id);
            return Ok(new { mediator.Id, mediator.FullName, mediator.IsActive, message = "Mediador desactivado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH /api/mediator/{id}/activate — Solo admin reactiva
    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Activate(string id)
    {
        try
        {
            var mediator = await _mediatorService.Activate(id);
            return Ok(new { mediator.Id, mediator.FullName, mediator.IsActive, message = "Mediador activado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
