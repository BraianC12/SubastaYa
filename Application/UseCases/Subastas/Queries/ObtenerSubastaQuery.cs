using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Subastas.Queries
{
    public record ObtenerSubastaQuery(int id) : IRequest<AuctionDetailDto>;
}
