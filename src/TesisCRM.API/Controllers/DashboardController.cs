using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Dashboard;
using TesisCRM.API.Repositories;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardRepository _repository;
    public DashboardController(DashboardRepository repository) => _repository = repository;

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen() => Ok(ApiResponse<DashboardResumenDto>.Ok(await _repository.GetResumenAsync()));
}
