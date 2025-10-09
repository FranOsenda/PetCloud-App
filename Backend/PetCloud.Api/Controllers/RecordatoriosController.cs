using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using PetCloud.Api.Models;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordatoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public RecordatoriosController(ApplicationDbContext db) { _db = db; }

        [HttpGet("mascota/{mascotaId}")]
        public async Task<IActionResult> GetByMascota(int mascotaId)
        {
            var list = await _db.Recordatorios.Where(r => r.MascotaId == mascotaId).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var rec = await _db.Recordatorios.FindAsync(id);
            if (rec == null) return NotFound();
            return Ok(rec);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Recordatorio dto)
        {
            _db.Recordatorios.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Recordatorio dto)
        {
            var r = await _db.Recordatorios.FindAsync(id);
            if (r == null) return NotFound();
            r.Titulo = dto.Titulo;
            r.Descripcion = dto.Descripcion;
            r.FechaProgramada = dto.FechaProgramada;
            r.Estado = dto.Estado;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Recordatorios.FindAsync(id);
            if (r == null) return NotFound();
            _db.Recordatorios.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}