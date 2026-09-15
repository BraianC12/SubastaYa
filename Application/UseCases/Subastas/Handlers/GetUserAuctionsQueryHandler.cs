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
    public class GetUserAuctionsQueryHandler : IGetUserAuctionsQueryHandler
    {
        private readonly ISubastaRepository _subastaRepository;

        public GetUserAuctionsQueryHandler(ISubastaRepository subastaRepository)
        {
            _subastaRepository = subastaRepository;
        }

        public async Task<List<SellerAuctionDto>> Handle(GetUserAuctionsQuery request)
        {
            var auctions = await _subastaRepository.GetBySellerIdAsync(request.Id);

            var auctionsDto = auctions.Select(s => 
            {
                var pujaMasAlta = s.Pujas != null && s.Pujas.Any() ? s.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault() : null;
                bool estaFinalizada = s.Estado == "FINALIZADA" && pujaMasAlta != null;

                return new SellerAuctionDto
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Url_Imagen = s.Url_Imagen,
                    Precio_Base = s.Precio_Base,
                    Fecha_Inicio = s.Fecha_Inicio,
                    Fecha_Fin = s.Fecha_Fin,
                    Estado = s.Estado,
                    CantidadPujas = s.Pujas != null ? s.Pujas.Count() : 0,
                    PujaMasAlta = pujaMasAlta?.Monto,
                    GanadorId = estaFinalizada ? pujaMasAlta.Comprador_Id : null,
                    GanadorNombre = estaFinalizada ? pujaMasAlta?.Comprador?.Nombre : null,
                };
            }).ToList();

            return auctionsDto;
        }
    }
}
