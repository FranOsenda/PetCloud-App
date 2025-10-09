using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCloud.Api.Data;
using PetCloud.Api.Models;

namespace PetCloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GastosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public GastosController(ApplicationDbContext db) { _db = db; }

        [Authorize]
        [HttpGet("mascota/{mascotaId}")]
        public async Task<IActionResult> GetByMascota(int mascotaId)
        {
            var gastos = await _db.Gastos.Where(g => g.MascotaId == mascotaId).ToListAsync();
            return Ok(gastos);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Gasto dto)
        {
            _db.Gastos.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(null, new { id = dto.Id }, dto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Gasto dto)
        {
            var gasto = await _db.Gastos.FindAsync(id);
            if (gasto == null) return NotFound();

            gasto.Monto = dto.Monto;
            gasto.Descripcion = dto.Descripcion;
            gasto.Fecha = dto.Fecha;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var gasto = await _db.Gastos.FindAsync(id);
            if (gasto == null) return NotFound();
            _db.Gastos.Remove(gasto);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}