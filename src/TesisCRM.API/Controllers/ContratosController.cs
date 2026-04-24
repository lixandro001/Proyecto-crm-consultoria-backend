using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Contratos;
using TesisCRM.API.Repositories;
using TesisCRM.API.Services;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/contratos")]
[Authorize]
public class ContratosController : ControllerBase
{
    private readonly ContratoRepository _repository;
    private readonly ContratoWordService _wordService;
    private readonly ContratoPdfService _pdfService;

    public ContratosController(ContratoRepository repository, ContratoWordService wordService, ContratoPdfService pdfService)
    {
        _repository = repository;
        _wordService = wordService;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<ContratoDto>>.Ok(await _repository.ListAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var data = await _repository.GetByIdAsync(id);
        return data is null ? NotFound(ApiResponse<string>.Fail("Contrato no encontrado.")) : Ok(ApiResponse<ContratoDto>.Ok(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContratoCreateRequest request)
        => Ok(ApiResponse<object>.Ok(new { Id = await _repository.CreateAsync(request) }, "Contrato registrado correctamente."));

    [HttpGet("descargar-word/{contratoId:int}")]
    public async Task<IActionResult> DescargarWord(int contratoId)
    {
        var data = await _repository.ObtenerDataPlantillaAsync(contratoId);

        if (data is null)
            return NotFound(ApiResponse<string>.Fail("Contrato no encontrado."));

        if (string.IsNullOrWhiteSpace(data.RutaPlantillaWord))
            return BadRequest(ApiResponse<string>.Fail("El contrato no tiene plantilla asociada."));

        var bytes = _wordService.GenerarWordDesdePlantilla(data.RutaPlantillaWord, data, out var rutaWord);
        await _repository.UpdateRutaWordAsync(contratoId, rutaWord);

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"Contrato_{data.ClienteNombreCompleto.Replace(" ", "_")}.docx"
        );
    }

    [HttpPost("generar-pdf")]
    public async Task<IActionResult> GenerarPdf([FromBody] ContratoPdfRequest request)
        => Ok(ApiResponse<object>.Ok(new { Ruta = await _pdfService.GenerarPdfAsync(request.ContratoId) }, "PDF generado correctamente."));
}
