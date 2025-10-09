using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using PetCloud.Api.Models;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistorialesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public HistorialesController(ApplicationDbContext db) { _db = db; }

        [HttpGet("mascota/{mascotaId}")]
        public async Task<IActionResult> GetByMascota(int mascotaId)
        {
            var list = await _db.HistorialesMedicos.Where(h => h.MascotaId == mascotaId).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var h = await _db.HistorialesMedicos.FindAsync(id);
            if (h == null) return NotFound();
            return Ok(h);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HistorialMedico dto)
        {
            _db.HistorialesMedicos.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HistorialMedico dto)
        {
            var h = await _db.HistorialesMedicos.FindAsync(id);
            if (h == null) return NotFound();
            h.Tipo = dto.Tipo;
            h.Observaciones = dto.Observaciones;
            h.Tratamiento = dto.Tratamiento;
            h.VeterinarioId = dto.VeterinarioId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var h = await _db.HistorialesMedicos.FindAsync(id);
            if (h == null) return NotFound();
            _db.HistorialesMedicos.Remove(h);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}