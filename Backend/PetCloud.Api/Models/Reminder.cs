namespace PetCloud.Api.Models
{
    public enum EstadoRecordatorio
    {
        Pendiente,
        Completado,
        Cancelado
    }

    public class Recordatorio
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaProgramada { get; set; }
        public EstadoRecordatorio Estado { get; set; } = EstadoRecordatorio.Pendiente;
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }
    }
}