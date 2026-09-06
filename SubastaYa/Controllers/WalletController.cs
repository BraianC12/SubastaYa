using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.DTOs;
using Application.UseCases.Billeteras.Commands;
using Application.UseCases.Billeteras.Queries;
using Application.UseCases.Billeteras.Handlers;
using Application.Interfaces;

namespace SubastaYa.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IListarBilleteraQueryHandler _listarHandler;
        private readonly IDepositCommandHandler _depositHandler;

        public WalletController(
            IListarBilleteraQueryHandler listarHandler,
            IDepositCommandHandler depositHandler)
        {
            _listarHandler = listarHandler;
            _depositHandler = depositHandler;
        }

        [HttpGet("{usuarioId}/balance")]
        public async Task<ActionResult<WalletBalanceDto>> GetBalance(int usuarioId)
        {
            var query = new ListarBilleteraQuery { Usuario_Id = usuarioId };
            var resultado = await _listarHandler.Handle(query);
            return Ok(resultado);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositCommand command)
        {
            decimal nuevoSaldo = await _depositHandler.Handle(command);
            return Ok(new
            {
                mensaje = "Depósito realizado con éxito",
                saldoActualizado = nuevoSaldo
            });
        }
    }
}