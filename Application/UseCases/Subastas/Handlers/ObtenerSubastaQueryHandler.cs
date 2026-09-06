using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Handlers
{
    public class ObtenerSubastaQueryHandler : IObtenerSubastaQueryHandler
    {
        private readonly ISubastaRepository _repository;

        public ObtenerSubastaQueryHandler(ISubastaRepository subastaRepository)
        {
            _repository = subastaRepository;
        }

        public async Task<AuctionDetailDto> Handle(ObtenerSubastaQuery request)
        {
            var subasta = await _repository.Obtener(request.id);

            if (subasta == null)
            {
                throw new Exception("Subasta no encontrada");
            }

            var subastaDto = new AuctionDetailDto
            {
                Id = subasta.Id,
                Titulo = subasta.Titulo,
                Descripcion = subasta.Descripcion,
                Url_Imagen = subasta.Url_Imagen,
                Precio_Base = subasta.Precio_Base,
                Incremento_Minimo = subasta.Incremento_Minimo,
                Fecha_Inicio = subasta.Fecha_Inicio,
                Fecha_Fin = subasta.Fecha_Fin,
                Estado = subasta.Estado,
                Categoria_Id = subasta.Categoria_Id,
                Vendedor_Id = subasta.Vendedor_Id,
                Puja_Actual = subasta.Pujas.OrderByDescending(p => p.Monto).Select(p => (decimal?)p.Monto).FirstOrDefault()
            };

            return subastaDto;
        }
    }
}