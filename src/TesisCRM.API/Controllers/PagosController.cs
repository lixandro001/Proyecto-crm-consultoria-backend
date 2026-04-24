using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Pagos;
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/pagos")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly PagoRepository _repository;
    public PagosController(PagoRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<PagoDto>>.Ok(await _repository.ListAsync()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PagoCreateRequest request)
        => Ok(ApiResponse<object>.Ok(new { Id = await _repository.CreateAsync(request) }, "Pago o deuda registrado correctamente."));
}
