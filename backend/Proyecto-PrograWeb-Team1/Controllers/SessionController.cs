using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Services;

namespace Proyecto_PrograWeb_Team1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly SessionService _sessionService;

    public SessionController(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    // POST /api/session — Mediador programa una sesión
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var session = await _sessionService.Create(dto, userId);
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/session/case/{caseId} — Ver sesiones de un caso
    [HttpGet("case/{caseId}")]
    public async Task<IActionResult> GetByCase(string caseId)
    {
        try
        {
            var sessions = await _sessionService.GetByCase(caseId);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/session/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var session = await _sessionService.GetById(id);
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/session/{id}/complete — Mediador completa la sesión
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(string id, [FromBody] CompleteSessionDto dto)
    {
        try
        {
            var session = await _sessionService.CompleteSession(id, dto);
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/session/{id}/confirm — Parte confirma asistencia
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmAttendance(string id, [FromQuery] bool isReporter = true)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var session = await _sessionService.ConfirmAttendance(id, userId, isReporter);
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
