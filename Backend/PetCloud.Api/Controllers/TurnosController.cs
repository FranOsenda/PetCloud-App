using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using PetCloud.Api.Models;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TurnosController(ApplicationDbContext db) { _db = db; }

        [HttpGet("mascota/{mascotaId}")]
        public async Task<IActionResult> GetByMascota(int mascotaId)
        {
            var list = await _db.Turnos.Where(t => t.MascotaId == mascotaId).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var t = await _db.Turnos.FindAsync(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Turno dto)
        {
            _db.Turnos.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Turno dto)
        {
            var t = await _db.Turnos.FindAsync(id);
            if (t == null) return NotFound();
            t.Fecha = dto.Fecha;
            t.Motivo = dto.Motivo;
            t.Estado = dto.Estado;
            t.VeterinarioId = dto.VeterinarioId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.Turnos.FindAsync(id);
            if (t == null) return NotFound();
            _db.Turnos.Remove(t);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}