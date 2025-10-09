namespace PetCloud.Api.Models
{
    public class Vacuna
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Dosis { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Lugar { get; set; }
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }
    }
}