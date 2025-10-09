namespace PetCloud.Api.Models
{
    public enum TipoConsulta
    {
        General,
        Emergencia,
        Vacunacion,
        Control
    }

    public class HistorialMedico
    {
        public int Id { get; set; }
        public TipoConsulta Tipo { get; set; }
        public string? Observaciones { get; set; }
        public string? Tratamiento { get; set; }
        public int? VeterinarioId { get; set; }
        public Veterinario? Veterinario { get; set; }
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }
    }
}