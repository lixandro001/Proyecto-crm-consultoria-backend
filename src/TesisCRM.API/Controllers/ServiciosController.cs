using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Servicios;
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/servicios")]
[Authorize]
public class ServiciosController : ControllerBase
{
    private readonly ServicioRepository _repository;
    public ServiciosController(ServicioRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(await _repository.ListAsync()));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ServicioCreateRequest request)
        => Ok(ApiResponse<object>.Ok(new { Id = await _repository.CreateAsync(request) }, "Servicio registrado correctamente."));
}
