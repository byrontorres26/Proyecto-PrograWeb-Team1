using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Services;

namespace Proyecto_PrograWeb_Team1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgreementController : ControllerBase
{
    private readonly AgreementService _agreementService;

    public AgreementController(AgreementService agreementService)
    {
        _agreementService = agreementService;
    }

    // POST /api/agreement — Mediador redacta un acuerdo
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAgreementDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var agreement = await _agreementService.Create(dto, userId);
            return Ok(agreement);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/agreement/case/{caseId} — Ver acuerdos de un caso
    [HttpGet("case/{caseId}")]
    public async Task<IActionResult> GetByCase(string caseId)
    {
        try
        {
            var agreements = await _agreementService.GetByCase(caseId);
            return Ok(agreements);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/agreement/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var agreement = await _agreementService.GetById(id);
            return Ok(agreement);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/agreement/{id}/confirm — Parte confirma el acuerdo
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var agreement = await _agreementService.ConfirmAgreement(id, userId);
            return Ok(new
            {
                agreement.Id,
                agreement.Status,
                agreement.ConfirmedByReporter,
                agreement.ConfirmedByRespondent,
                message = agreement.Status == "formalizado"
                    ? "¡Acuerdo formalizado con éxito! Ambas partes confirmaron."
                    : "Confirmación registrada. Esperando confirmación de la otra parte."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/agreement/compliance — Reportar cumplimiento/incumplimiento
    [HttpPost("compliance")]
    public async Task<IActionResult> ReportCompliance([FromBody] ReportComplianceDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var agreement = await _agreementService.ReportCompliance(dto, userId);
            return Ok(new
            {
                message = $"Cumplimiento reportado como: {dto.ComplianceStatus}",
                agreement.Id,
                Points = agreement.Points
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
