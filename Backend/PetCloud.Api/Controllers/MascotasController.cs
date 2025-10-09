using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using PetCloud.Api.Models;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public MascotasController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var mascotas = await _db.Mascotas.Include(m => m.Dueno).ToListAsync();
            return Ok(mascotas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var mascota = await _db.Mascotas.Include(m => m.Dueno).SingleOrDefaultAsync(m => m.Id == id);
            if (mascota == null) return NotFound();
            return Ok(mascota);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Mascota dto)
        {
            _db.Mascotas.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Mascota dto)
        {
            var mascota = await _db.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();

            mascota.Nombre = dto.Nombre;
            mascota.Especie = dto.Especie;
            mascota.Raza = dto.Raza;
            mascota.Sexo = dto.Sexo;
            mascota.FechaNacimiento = dto.FechaNacimiento;
            mascota.Peso = dto.Peso;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mascota = await _db.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();
            _db.Mascotas.Remove(mascota);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}