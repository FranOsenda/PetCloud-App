using Microsoft.AspNetCore.Mvc;
using PetCloud.Api.Data;
using PetCloud.Api.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (_db.Usuarios.Any(u => u.CorreoElectronico == dto.Email))
                return BadRequest("El correo ya está en uso");

            var usuario = new Usuario
            {
                Nombre = dto.FirstName,
                Apellido = dto.LastName,
                CorreoElectronico = dto.Email,
                HashContrasena = ComputeSha256Hash(dto.Password),
                Rol = dto.Role == "veterinario" ? Rol.Veterinario : Rol.Dueno
            };

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();

            if (usuario.Rol == Rol.Dueno)
            {
                var dueno = new Dueno { UsuarioId = usuario.Id };
                _db.Duenos.Add(dueno);
                await _db.SaveChangesAsync();
            }
            else
            {
                var vet = new Veterinario { UsuarioId = usuario.Id };
                _db.Veterinarios.Add(vet);
                await _db.SaveChangesAsync();
            }

            return CreatedAtAction(null, new { id = usuario.Id }, new { usuario.Id, usuario.CorreoElectronico });
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var usuario = _db.Usuarios.SingleOrDefault(u => u.CorreoElectronico == dto.Email);
            if (usuario == null) return Unauthorized("Credenciales inválidas");

            var hash = ComputeSha256Hash(dto.Password);
            if (usuario.HashContrasena != hash) return Unauthorized("Credenciales inválidas");

            // Generar token
            var jwtSection = HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, "Configuración JWT inválida en el servidor");
            }

            var claims = new[] {
                new System.Security.Claims.Claim("id", usuario.Id.ToString()),
                new System.Security.Claims.Claim("email", usuario.CorreoElectronico),
                new System.Security.Claims.Claim("rol", usuario.Rol.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer,
              audience,
              claims,
              expires: DateTime.Now.AddMinutes(jwtSection.GetValue<int>("ExpiresMinutes")),
              signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "dueno"; // "dueno" o "veterinario"
    }
}