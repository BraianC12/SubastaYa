using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Domain;
using Application.UsesCases.Billeteras.Handlers;
using Infraestructure.Repositories;
using System.Threading.Tasks;
using Application.UseCases.Billeteras.Commands;

namespace SubastaYa.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ListarBilleterasQueryHandler _listar;
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _listar = new ListarBilleterasQueryHandler(new BilleteraRepository(context));
        }

        [HttpGet("balance")]
        public async Task<ActionResult<IEnumerable<WalletBalanceDto>>> GetBalance()
        {
            var billeteras = await _listar.Handle();

            return Ok(billeteras);
            _mediator = mediator;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
        {
            decimal nuevoSaldo = await _mediator.Send(command);

            return Ok(new
            {
                mensaje = "Depósito realizado con éxito",
                saldoActualizado = nuevoSaldo
            });
        }
    }
}