using Application.DTOs;
using Application.UseCases.Billeteras.Handlers;
using Application.UseCases.Billeteras.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IListarTransaccionesQueryHandler
    {
        Task<IEnumerable<TransaccionLedgerDto>> Handle(ListarTransaccionesQuery request);
    }
}
