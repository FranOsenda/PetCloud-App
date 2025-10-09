namespace PetCloud.Api.Models
{
    public enum EstadoTurno
    {
        Pendiente,
        Completado,
        Cancelado
    }

    public class Turno
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }
        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;
        public int? VeterinarioId { get; set; }
        public Veterinario? Veterinario { get; set; }
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }
    }
}