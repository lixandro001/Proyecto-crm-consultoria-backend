using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.ClienteServicios;
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/cliente-servicios")]
[Authorize]
public class ClienteServiciosController : ControllerBase
{
    private readonly ClienteServicioRepository _repository;
    public ClienteServiciosController(ClienteServicioRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<ClienteServicioDto>>.Ok(await _repository.ListAsync()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteServicioCreateRequest request)
        => Ok(ApiResponse<object>.Ok(new { Id = await _repository.CreateAsync(request) }, "Servicio asignado al cliente correctamente."));
}
