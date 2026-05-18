using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zeiterfassung.Application.Dtos.Auth;
using Zeiterfassung.Application.UseCases.Auth;
using Zeiterfassung.Infrastructure;

namespace Zeiterfassung.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly GetUserProfileUseCase _getUserProfileUseCase;
    private readonly ZeiterfassungDbContext _dbContext;

    public AuthController(
        RegisterUseCase registerUseCase,
        LoginUseCase loginUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase,
        GetUserProfileUseCase getUserProfileUseCase,
        ZeiterfassungDbContext dbContext)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _getUserProfileUseCase = getUserProfileUseCase;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _registerUseCase.ExecuteAsync(request);
            await _dbContext.SaveChangesAsync();
            return Created("/auth/me", response);
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { error = "Benutzername existiert bereits oder Passwort ungültig." });
        }
        catch
        {
            return Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(request);
            await _dbContext.SaveChangesAsync();
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized();
        }
        catch
        {
            return Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _refreshTokenUseCase.ExecuteAsync(request);
            await _dbContext.SaveChangesAsync();
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized();
        }
        catch
        {
            return Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
        }
    }

    [Authorize(Policy = "AuthenticatedUser")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        try
        {
            await _logoutUseCase.ExecuteAsync(request.RefreshToken);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Erfolgreich abgemeldet." });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { error = "Ungültiger Refresh Token." });
        }
        catch
        {
            return Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
        }
    }

    [Authorize(Policy = "AuthenticatedUser")]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var response = await _getUserProfileUseCase.ExecuteAsync(userIdClaim.Value);
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound(new { error = "Benutzer nicht gefunden." });
        }
        catch
        {
            return Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
        }
    }
}
