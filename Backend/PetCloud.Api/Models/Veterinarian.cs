namespace PetCloud.Api.Models
{
    public class Veterinario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public string? Dni { get; set; }
        public string? Matricula { get; set; }
        public string? Institucion { get; set; }
        public string? Telefono { get; set; }

        public ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}