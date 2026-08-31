namespace Application.DTOs
{
    public class CreateAuctionDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Url_Imagen { get; set; }
        public decimal Precio_Base { get; set; }
        public decimal Incremento_Minimo { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public int Categoria_Id { get; set; }
        public int Vendedor_Id { get; set; }
    }
}