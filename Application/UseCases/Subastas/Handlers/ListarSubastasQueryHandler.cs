using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using Domain;

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

            if(subastas == null)
            {
                throw new Exception();
            }

            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                subastas = subastas.Where(s => s.Estado == request.Estado).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Categoria))
            {
                subastas = subastas.Where(s => s.Categoria.Nombre == request.Categoria).ToList();
            }

            var subastasDto = subastas.Select(s => new AuctionDto
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
                Puja_Actual = s.Pujas.OrderByDescending(p => p.Monto).Select(p => (decimal?)p.Monto).FirstOrDefault(),
                Vendedor_Id = s.Vendedor_Id
            });

            if (request.Ordenar.HasValue)
            {
                switch (request.Ordenar.Value)
                {
                    case CriterioOrden.Fecha:
                        subastasDto = subastasDto.OrderBy(s => s.Fecha_Fin);
                        break;
                    case CriterioOrden.Precio:
                        subastasDto = subastasDto.OrderByDescending(s => s.Puja_Actual);
                        break;
                    default:
                        throw new Exception();
                }
            }

            return subastasDto;
        }
    }
}