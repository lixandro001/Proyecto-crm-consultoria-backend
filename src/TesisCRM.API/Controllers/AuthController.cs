using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesisCRM.API.Models.Common;

using TesisCRM.API.Models.Auth;
using TesisCRM.API.Repositories;
using TesisCRM.API.Services;

namespace TesisCRM.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthRepository _authRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly PasswordService _passwordService;

    public AuthController(AuthRepository authRepository, JwtTokenService jwtTokenService, PasswordService passwordService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authRepository.GetByUsernameAsync(request.Username);
        if (user is null || !user.IsActive)
            return Unauthorized(ApiResponse<string>.Fail("Usuario no encontrado o inactivo."));

        var valid = user.PasswordHash == request.Password || _passwordService.Verify(request.Password, user.PasswordHash);
        if (!valid)
            return Unauthorized(ApiResponse<string>.Fail("Credenciales inválidas."));

        var token = _jwtTokenService.Generate(user);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.RoleCode,
            Token = token
        }, "Login correcto."));
    }
}
