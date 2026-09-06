
namespace Application.UseCases.Subastas.Queries
{
    public class ListarSubastasQuery
    {
        public string? Estado { get; set; }
        public string? Categoria { get; set; }
        public string? OrdenarPorPrecio { get; set; }
        public string? OrdenarPorFecha { get; set; }

    }
}