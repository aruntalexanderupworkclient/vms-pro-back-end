using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VMS.Application.DTOs;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;

namespace VMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            // 1. Validate the Google ID Token (official Google library)
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            // 2. payload contains verified user info
            // Here you can: find/create user in your DB, assign roles, etc.

            // 3. Issue your own app JWT
            var token = GenerateJwt(payload.Email, payload.Name, payload.Subject);

            return Ok(new { token });
        }
        catch (InvalidJwtException)
        {
            return Unauthorized("Invalid Google token.");
        }
    }

    private string GenerateJwt(string email, string name, string googleId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name),
            new Claim("google_id", googleId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.GetByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(ApiResponse<LoginResponseDto>.FailResponse("Invalid email or password."));

        var token = GenerateJwtToken(user);
        var refreshToken = Guid.NewGuid().ToString("N");

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
            User = user
        };

        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful."));
    }

    /// <summary>
    /// DEV-ONLY: Returns a mock JWT for testing without real credentials.
    /// Do not use in production.
    /// </summary>
    [HttpPost("demo-login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> DemoLogin()
    {
        var user = await _userService.GetByEmailAsync("admin@vms.com");
        if (user == null)
            return NotFound(ApiResponse<LoginResponseDto>.FailResponse("Demo user not found. Ensure InMemory mode with seed data."));

        var token = GenerateJwtToken(user);
        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
            User = user
        };

        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Demo login successful. Do not use in production."));
    }

    [HttpPost("sso-callback")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<LoginResponseDto>> SsoCallback([FromBody] SsoCallbackDto dto)
    {
        _logger.LogInformation("SSO callback received with code: {Code}", dto.Code);
        return Ok(ApiResponse<LoginResponseDto>.FailResponse("SSO integration requires external IdP configuration. Configure SsoSettings in appsettings.json."));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<LoginResponseDto>> Refresh([FromBody] RefreshTokenDto dto)
    {
        var response = new LoginResponseDto
        {
            Token = dto.Token,
            RefreshToken = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes())
        };

        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed."));
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult<ApiResponse<object>> Logout()
    {
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Logged out successfully."));
    }

    private string GenerateJwtToken(UserDto user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "VMS-Default-Secret-Key-For-Development-Only-256-Bits!!";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.RoleName ?? "Viewer"),
            new("organisationId", user.OrganisationId?.ToString() ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "VMS.API",
            audience: jwtSettings["Audience"] ?? "VMS.Client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetExpiryMinutes()
    {
        var value = _configuration["JwtSettings:ExpiryMinutes"];
        return int.TryParse(value, out var minutes) ? minutes : 60;
    }
}


public class GoogleLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
}
