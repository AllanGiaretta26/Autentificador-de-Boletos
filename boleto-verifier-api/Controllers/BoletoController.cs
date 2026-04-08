using BoletoVerifier.Models;
using BoletoVerifier.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoletoVerifier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoletoController : ControllerBase
{
    private readonly BoletoParser _parser;
    private readonly EmailService _emailService;

    public BoletoController(BoletoParser parser, EmailService emailService)
    {
        _parser = parser;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessarBoleto([FromBody] BoletoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CodigoBarras))
            return BadRequest(new { erro = "Código de barras é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.EmailDestino))
            return BadRequest(new { erro = "E-mail de destino é obrigatório." });

        var info = _parser.Extrair(request.CodigoBarras);

        try
        {
            await _emailService.EnviarAsync(request.EmailDestino, info);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Boleto processado, mas falha ao enviar e-mail.", detalhe = ex.Message });
        }

        return Ok(new
        {
            mensagem = "Boleto processado e e-mail enviado com sucesso!",
            dados = info
        });
    }
}