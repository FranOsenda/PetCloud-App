namespace PetCloud.Api.Models
{
    public class Gasto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int MascotaId { get; set; }
        public Mascota? Mascota { get; set; }
    }
}