
namespace Application.UseCases.Subastas.Queries
{
    public enum CriterioOrden
    {
        Fecha,
        Precio
    }

    public class ListarSubastasQuery
    {
        public string? Estado { get; set; }
        public string? Categoria { get; set; }

        public CriterioOrden? Ordenar { get; set; }

    }
}