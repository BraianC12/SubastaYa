using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Handlers
{
    public class ListarSubastasQueryHandler : IRequestHandler<ListarSubastasQuery, IEnumerable<AuctionDto>>
    {
        private readonly ISubastaRepository _repository;

        public ListarSubastasQueryHandler(ISubastaRepository subastaRepository)
        {
            _repository = subastaRepository;
        }
        
        public async Task<IEnumerable<AuctionDto>> Handle(ListarSubastasQuery request, CancellationToken cancellationToken)
        {
            var subastas = await _repository.Listar();

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
                Categoria_Id = s.Categoria_Id,
                Vendedor_Id = s.Vendedor_Id
            });

            return subastasDto;
        }
    }
}