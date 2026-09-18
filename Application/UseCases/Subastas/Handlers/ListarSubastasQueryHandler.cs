using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using Domain;
using Domain.Exceptions;

namespace Application.UseCases.Handlers
{
    public class ListarSubastasQueryHandler: IListarSubastasQueryHandler
    {
        private readonly ISubastaRepository _repository;

        public ListarSubastasQueryHandler(ISubastaRepository subastaRepository)
        {
            _repository = subastaRepository;
        }
        
        public async Task<IEnumerable<AuctionDto>> Handle(ListarSubastasQuery request)
        {
            var subastas = _repository.Listar();

            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                subastas = subastas.Where(s => s.Estado == request.Estado.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(request.Categoria))
            {
                subastas = subastas.Where(s => s.Categoria.Nombre == request.Categoria);
            }

            if (!string.IsNullOrWhiteSpace(request.Busqueda))
            {
                subastas = subastas.Where(s => s.Titulo.Contains(request.Busqueda));
            }

            if (request.Ordenar.HasValue)
            {
                switch (request.Ordenar.Value)
                {
                    case CriterioOrden.Fecha:
                        subastas = subastas.OrderBy(s => s.Fecha_Fin);
                        break;
                    case CriterioOrden.Precio:
                        subastas = subastas.OrderByDescending(s => s.Pujas.Max(p => (decimal?)p.Monto) ?? s.Precio_Base);
                        break;
                    default:
                        subastas = subastas.OrderBy(s => s.Id);
                        break;
                }
            }

            int pagina = request.Pagina < 1 ? 1 : request.Pagina;
            int tamaño = 6;

            var subastasPaginadas = subastas.Skip((pagina - 1) * tamaño).Take(tamaño).ToList();

            var subastasDto = subastasPaginadas.Select(s => new AuctionDto
            {
                Id = s.Id,
                Titulo = s.Titulo,
                Descripcion = s.Descripcion,
                Url_Imagen = s.Url_Imagen,
                Precio_Base = s.Precio_Base,
                Incremento_Minimo = s.Incremento_Minimo,
                Fecha_Inicio = s.Fecha_Inicio,
                Fecha_Fin = s.Fecha_Fin,
                Estado = s.Estado,
                Categoria = s.Categoria.Nombre,
                Puja_Actual = s.Pujas != null && s.Pujas.Any() ? s.Pujas.Max(p => p.Monto) : null,
                Vendedor_Id = s.Vendedor_Id
            });

            return subastasDto;
        }
    }
}