namespace PetCloud.Api.Models
{
    public enum Rol
    {
        Dueno,
        Veterinario,
        Admin
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string HashContrasena { get; set; } = string.Empty;
        public Rol Rol { get; set; } = Rol.Dueno;
    }
}