using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Plantillas;
using TesisCRM.API.Repositories;
using TesisCRM.API.Services;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/plantillas-contrato")]
[Authorize]
public class PlantillasContratoController : ControllerBase
{
    private readonly PlantillaContratoRepository _repository;
    private readonly FileStorageService _fileStorageService;

    public PlantillasContratoController(
        PlantillaContratoRepository repository,
        FileStorageService fileStorageService)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var data = await _repository.ListAsync();
        return Ok(ApiResponse<IEnumerable<PlantillaContratoDto>>.Ok(data));
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Upload([FromForm] PlantillaContratoUploadRequest request)
    {
        if (request.ArchivoWord is null || request.ArchivoWord.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Debe adjuntar un archivo Word."));

        var ruta = await _fileStorageService.SaveAsync(request.ArchivoWord, "PlantillasWord");
        var id = await _repository.CreateAsync(request.NombrePlantilla, ruta, request.Descripcion);

        return Ok(ApiResponse<object>.Ok(
            new { Id = id, Ruta = ruta },
            "Plantilla subida correctamente."
        ));
    }
}