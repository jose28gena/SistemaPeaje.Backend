using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Login de administrador. Las credenciales se leen de la configuración
    /// (Auth:AdminUsername / Auth:AdminPassword, p. ej. variables de entorno
    /// Auth__AdminUsername y Auth__AdminPassword). Si no están definidas, el login queda deshabilitado.
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var adminUser = _configuration["Auth:AdminUsername"];
        var adminPassword = _configuration["Auth:AdminPassword"];

        if (!string.IsNullOrEmpty(adminUser) && !string.IsNullOrEmpty(adminPassword)
            && SecureEquals(request.Username, adminUser)
            && SecureEquals(request.Password, adminPassword))
        {
            var token = GenerateJwtToken(request.Username);
            return Ok(new { token });
        }

        return Unauthorized("Credenciales inválidas");
    }

    /// <summary>Login de empleado: solo se emite token si el documento corresponde a un empleado activo.</summary>
    [HttpPost("login-empleado")]
    public async Task<IActionResult> LoginEmpleado([FromBody] LoginEmpleadoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NumeroDocumento))
            return Unauthorized("Número de documento requerido");

        var documento = request.NumeroDocumento.Trim();
        var empleados = await _unitOfWork.Repository<Empleado>()
            .GetAsync(e => e.NumeroDocumento == documento && e.EsActivo);

        if (empleados.Count == 0)
            return Unauthorized("Empleado no encontrado o inactivo");

        var token = GenerateJwtToken(documento, "Empleado");
        return Ok(new { token, empleado = documento });
    }

    // Comparación en tiempo constante para no filtrar información por diferencias de tiempo.
    private static bool SecureEquals(string? provided, string expected)
    {
        var a = SHA256.HashData(Encoding.UTF8.GetBytes(provided ?? string.Empty));
        var b = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        return CryptographicOperations.FixedTimeEquals(a, b);
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
