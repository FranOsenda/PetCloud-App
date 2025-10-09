namespace PetCloud.Api.Models
{
    public class Dueno
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Campos adicionales del dueño
        public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}