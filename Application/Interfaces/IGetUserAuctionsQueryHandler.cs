using Application.DTOs;
using Application.UseCases.Subastas.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGetUserAuctionsQueryHandler
    {
        Task<List<SellerAuctionDto>> Handle(GetUserAuctionsQuery request);
    }
}
