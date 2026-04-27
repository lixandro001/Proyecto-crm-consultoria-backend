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
    private readonly PlantillaContratoRepository _plantillaRepository;
    private readonly ContratoWordService _wordService;
    //private readonly ContratoPdfService _pdfService;


    public ContratosController(
     ContratoRepository repository,
     PlantillaContratoRepository plantillaRepository,
     ContratoWordService wordService
      )
    {
        _repository = repository;
        _plantillaRepository = plantillaRepository;
        _wordService = wordService;
        //_pdfService = pdfService;
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

        if (!data.PlantillaContratoId.HasValue)
            return BadRequest(ApiResponse<string>.Fail("El contrato no tiene plantilla asociada."));

        var plantilla = await _plantillaRepository.GetArchivoByIdAsync(data.PlantillaContratoId.Value);

        if (plantilla is null || plantilla.ArchivoWord is null || plantilla.ArchivoWord.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("No se encontró el archivo Word de la plantilla."));

        var bytes = _wordService.GenerarWordDesdePlantillaBytes(plantilla.ArchivoWord, data);

        var nombreArchivo = $"Contrato_{data.ClienteNombreCompleto.Replace(" ", "_")}.docx";

        await _repository.UpdateArchivoWordGeneradoAsync(contratoId, nombreArchivo, bytes);

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            nombreArchivo
        );
    }

    //[HttpPost("generar-pdf")]
    //public async Task<IActionResult> GenerarPdf([FromBody] ContratoPdfRequest request)
    //    => Ok(ApiResponse<object>.Ok(new { Ruta = await _pdfService.GenerarPdfAsync(request.ContratoId) }, "PDF generado correctamente."));


}
