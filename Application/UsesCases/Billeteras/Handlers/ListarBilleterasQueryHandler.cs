using Application.DTOs;
using Application.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UsesCases.Billeteras.Handlers
{
    public class ListarBilleterasQueryHandler
    {
        private readonly IBilleteraRepository _repository;

        public ListarBilleterasQueryHandler(IBilleteraRepository billeteraRepository)
        {
            _repository = billeteraRepository;
        }

        public async Task<IEnumerable<WalletBalanceDto>> Handle()
        {
            var billeteras = await _repository.Listar();

            var billeterasDto = billeteras.Select(b => new WalletBalanceDto
            {
                Usuario = b.Usuario.Nombre,
                Saldo_Total = b.Saldo_Total,
                Saldo_Retenido = b.Saldo_Retenido,
                Saldo_Disponible = b.Saldo_Disponible
            });

            return billeterasDto;
        }
    }
}
