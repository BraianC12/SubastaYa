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
            var subastas = await _repository.Listar();

            if(subastas == null || !subastas.Any())
            {
                throw new NotFoundException("Actualmente no hay subastas disponibles");
            }

            var query = subastas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                query = query.Where(s => s.Estado == request.Estado);
            }

            if (!string.IsNullOrWhiteSpace(request.Categoria))
            {
                query = query.Where(s => s.Categoria.Nombre == request.Categoria);
            }

            if (request.Ordenar.HasValue)
            {
                switch (request.Ordenar.Value)
                {
                    case CriterioOrden.Fecha:
                        query = query.OrderBy(s => s.Fecha_Fin);
                        break;
                    case CriterioOrden.Precio:
                        query = query.OrderByDescending(s => s.Pujas != null && s.Pujas.Any() ? s.Pujas.Max(p => p.Monto) : s.Precio_Base);
                        break;
                    default:
                        throw new Exception();
                }
            }

            int pagina = request.Pagina < 1 ? 1 : request.Pagina;

            query = query.Skip((pagina - 1) * 2).Take(3).ToList(); 

            var subastasDto = query.Select(s => new AuctionDto
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
                Puja_Actual = s.Pujas != null && s.Pujas.Any() ? s.Pujas.Max(p => p.Monto) : s.Precio_Base,
                Vendedor_Id = s.Vendedor_Id
            });

            return subastasDto;
        }
    }
}