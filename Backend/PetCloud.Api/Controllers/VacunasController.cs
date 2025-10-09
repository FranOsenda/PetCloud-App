using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using PetCloud.Api.Models;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacunasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public VacunasController(ApplicationDbContext db) { _db = db; }

        [HttpGet("mascota/{mascotaId}")]
        public async Task<IActionResult> GetByMascota(int mascotaId)
        {
            var list = await _db.Vacunas.Where(v => v.MascotaId == mascotaId).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _db.Vacunas.FindAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vacuna dto)
        {
            _db.Vacunas.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Vacuna dto)
        {
            var v = await _db.Vacunas.FindAsync(id);
            if (v == null) return NotFound();
            v.Nombre = dto.Nombre;
            v.Dosis = dto.Dosis;
            v.Tipo = dto.Tipo;
            v.Fecha = dto.Fecha;
            v.Lugar = dto.Lugar;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _db.Vacunas.FindAsync(id);
            if (v == null) return NotFound();
            _db.Vacunas.Remove(v);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}