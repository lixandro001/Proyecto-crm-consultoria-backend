using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Users;
using TesisCRM.API.Repositories;
using TesisCRM.API.Services;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _repository;
    private readonly PasswordService _passwordService;

    public UsersController(UserRepository repository, PasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    [HttpGet]
    public async Task<IActionResult> List() => Ok(ApiResponse<IEnumerable<UserDto>>.Ok(await _repository.ListAsync()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
    {
        var id = await _repository.CreateAsync(request.Username, request.FullName, _passwordService.Hash(request.Password), request.RoleCode.ToUpperInvariant());
        return Ok(ApiResponse<object>.Ok(new { Id = id }, "Usuario registrado correctamente."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateRequest request)
    {
        if (id != request.Id) return BadRequest(ApiResponse<string>.Fail("El id no coincide."));
        await _repository.UpdateAsync(request);
        return Ok(ApiResponse<string>.Ok(null, "Usuario actualizado correctamente."));
    }
}
