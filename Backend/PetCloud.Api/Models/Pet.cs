using System.ComponentModel.DataAnnotations.Schema;

namespace PetCloud.Api.Models
{
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string Raza { get; set; } = string.Empty;
        public string? Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public double? Peso { get; set; }
        public int DuenoId { get; set; }
        public Dueno? Dueno { get; set; }
        public string? QR { get; set; }

        public ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();
        public ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
        public ICollection<Recordatorio> Recordatorios { get; set; } = new List<Recordatorio>();
        public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
    }
}