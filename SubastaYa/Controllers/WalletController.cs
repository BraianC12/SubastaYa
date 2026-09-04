using Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Domain;
using Application.UsesCases.Billeteras.Handlers;
using Infraestructure.Repositories;

namespace SubastaYa.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ListarBilleterasQueryHandler _listar;

        public WalletController(SubastaDbContext context)
        {
            _listar = new ListarBilleterasQueryHandler(new BilleteraRepository(context));
        }

        [HttpGet("balance")]
        public async Task<ActionResult<IEnumerable<WalletBalanceDto>>> GetBalance()
        {
            var billeteras = await _listar.Handle();

            return Ok(billeteras);
        }
    }
}
