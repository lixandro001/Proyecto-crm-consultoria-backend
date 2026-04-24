using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Clientes;
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly ClienteRepository _repository;
    public ClientesController(ClienteRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<ClienteDto>>.Ok(await _repository.ListAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var data = await _repository.GetByIdAsync(id);
        return data is null ? NotFound(ApiResponse<string>.Fail("Cliente no encontrado.")) : Ok(ApiResponse<ClienteDto>.Ok(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteCreateRequest request)
        => Ok(ApiResponse<object>.Ok(new { Id = await _repository.CreateAsync(request) }, "Cliente registrado correctamente."));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateRequest request)
    {
        if (id != request.Id) return BadRequest(ApiResponse<string>.Fail("El id no coincide."));
        await _repository.UpdateAsync(request);
        return Ok(ApiResponse<string>.Ok(null, "Cliente actualizado correctamente."));
    }
}
