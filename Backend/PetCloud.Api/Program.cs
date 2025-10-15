using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PetCloud.Api.Models;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=PetCloudDb;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key")
    ?? throw new InvalidOperationException("Falta configuración Jwt:Key en appsettings.json");
var issuer = jwtSection.GetValue<string>("Issuer")
    ?? throw new InvalidOperationException("Falta configuración Jwt:Issuer en appsettings.json");
var audience = jwtSection.GetValue<string>("Audience")
    ?? throw new InvalidOperationException("Falta configuración Jwt:Audience en appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed initial data (idempotent)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    static string Hash(string raw)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    // Seed Veterinario
    var vetEmail = "vet@demo.com";
    if (!db.Usuarios.Any(u => u.CorreoElectronico == vetEmail))
    {
        var vetUser = new Usuario
        {
            Nombre = "Vete",
            Apellido = "Demo",
            CorreoElectronico = vetEmail,
            HashContrasena = Hash("Vet1234!"),
            Rol = Rol.Veterinario
        };
        db.Usuarios.Add(vetUser);
        db.SaveChanges();
        db.Veterinarios.Add(new Veterinario { UsuarioId = vetUser.Id });
        db.SaveChanges();
    }

    // Seed Dueño + Mascota
    var duenoEmail = "dueno@demo.com";
    if (!db.Usuarios.Any(u => u.CorreoElectronico == duenoEmail))
    {
        var ownerUser = new Usuario
        {
            Nombre = "Dueno",
            Apellido = "Demo",
            CorreoElectronico = duenoEmail,
            HashContrasena = Hash("Dueno1234!"),
            Rol = Rol.Dueno
        };
        db.Usuarios.Add(ownerUser);
        db.SaveChanges();
        var dueno = new Dueno { UsuarioId = ownerUser.Id };
        db.Duenos.Add(dueno);
        db.SaveChanges();

        var mascota = new Mascota
        {
            Nombre = "Firulais",
            Especie = "Perro",
            Raza = "Mestizo",
            Sexo = "M",
            FechaNacimiento = DateTime.UtcNow.AddYears(-3),
            Peso = 18.2,
            DuenoId = dueno.Id
        };
        db.Mascotas.Add(mascota);
        db.SaveChanges();
    }
}
app.Run();