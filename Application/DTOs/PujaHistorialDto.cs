namespace Application.DTOs
{
    public class PujaHistorialDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string UsuarioAnonimo { get; set; }
        public DateTime FechaPuja { get; set; }
    }
}
