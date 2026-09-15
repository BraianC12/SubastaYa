using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Subastas.Handlers
{
    public class GetBuyerBidsQueryHandler : IGetBuyerBidsQueryHandler
    {
        private readonly ISubastaRepository _subastaRepository;

        public GetBuyerBidsQueryHandler(ISubastaRepository subastaRepository)
        {
            _subastaRepository = subastaRepository;
        }

        public async Task<List<MyBidAuctionDto>> Handle(GetBuyerBidsQuery request)
        {
            var auctions = await _subastaRepository.GetByBuyerIdAsync(request.Id);

            var auctionsDto = auctions.Select(s =>
            {
                var miPuja = s.Pujas.Where(p => p.Comprador_Id == request.Id).Max(p => p.Monto);
                var pujaLider = s.Pujas.Max(p => p.Monto);


                return new MyBidAuctionDto
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Url_Imagen = s.Url_Imagen,
                    Categoria = s.Categoria.Nombre,
                    Estado = s.Estado,
                    Fecha_Inicio = s.Fecha_Inicio,
                    Fecha_Fin = s.Fecha_Fin,
                    Precio_Base = s.Precio_Base,
                    Incremento_Minimo = s.Incremento_Minimo,
                    TuPuja = miPuja,
                    PujaLiderActual = pujaLider,
                    EsLider = s.Estado == "ACTIVA" && miPuja >= pujaLider,
                    EsGanador = s.Estado == "FINALIZADA" && miPuja >= pujaLider
                };
            }).ToList();

            return auctionsDto;
        }
    }
}
