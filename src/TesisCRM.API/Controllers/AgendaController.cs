using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Agenda;
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/agenda")]
[Authorize]
public class AgendaController : ControllerBase
{
    private readonly AgendaRepository _repository;
    public AgendaController(AgendaRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<AgendaEventoDto>>.Ok(await _repository.ListAsync()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AgendaCreateRequest request)
        => Ok(ApiResponse<object>.Ok(new { Id = await _repository.CreateAsync(request) }, "Evento agendado correctamente."));
}
