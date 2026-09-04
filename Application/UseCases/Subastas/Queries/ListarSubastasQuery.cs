using Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Application.UseCases.Subastas.Queries
{
    public class ListarSubastasQuery : IRequest<IEnumerable<AuctionDto>>
    {
    }
}