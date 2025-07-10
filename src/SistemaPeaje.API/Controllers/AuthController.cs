using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validación simple para desarrollo (en producción usar un servicio de autenticación)
        if (request.Username == "admin" && request.Password == "admin123")
        {
            var token = GenerateJwtToken(request.Username);
            return Ok(new { token });
        }

        return Unauthorized("Credenciales inválidas");
    }

    [HttpPost("login-empleado")]
    public IActionResult LoginEmpleado([FromBody] LoginEmpleadoRequest request)
    {
        // Validación simple para empleados (en producción validar contra base de datos)
        if (!string.IsNullOrEmpty(request.NumeroDocumento))
        {
            var token = GenerateJwtToken(request.NumeroDocumento, "Empleado");
            return Ok(new { token, empleado = request.NumeroDocumento });
        }

        return Unauthorized("Número de documento requerido");
    }

    private string GenerateJwtToken(string username, string role = "Admin")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginEmpleadoRequest
{
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Nombre { get; set; }
}
