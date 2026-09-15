using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Queries;
using Domain;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Billeteras.Handlers
{
    public class ListarTransaccionesQueryHandler : IListarTransaccionesQueryHandler
    {
        private readonly ITransaccionLedgerRepository _transaccionLedgerRepository;

        public ListarTransaccionesQueryHandler(ITransaccionLedgerRepository transaccionLedgerRepository)
        {
            _transaccionLedgerRepository = transaccionLedgerRepository;
        }

        public async Task<IEnumerable<TransaccionLedgerDto>> Handle(ListarTransaccionesQuery request)
        {
            var transacciones = await _transaccionLedgerRepository.GetByBilleteraIdAsync(request.Id);

            if (transacciones == null || !transacciones.Any())
            {
                throw new NotFoundException("Actualmente no hay transacciones realizadas");
            }

            var transaccionesDto = transacciones.Select(t => new TransaccionLedgerDto
            {
                Id = t.Id,
                Billetera_Id = t.Billetera_Id,
                Subasta_Id = t.Subasta_Id,
                Subasta_Titulo = t.Subasta != null ? t.Subasta.Titulo : null,
                Tipo = t.Tipo,
                Monto = t.Monto,
                Fecha = t.Fecha
            }).ToList();

            return transaccionesDto;
        }
    }
}
