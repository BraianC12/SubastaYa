using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Commands;
using Application.UseCases.Billeteras.Handlers;
using Application.UseCases.Billeteras.Queries;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SubastaYa.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IListarBilleteraQueryHandler _listarHandler;
        private readonly IDepositCommandHandler _depositHandler;
        private readonly IListarTransaccionesQueryHandler _transaccionesHandler;

        public WalletController(
            IListarBilleteraQueryHandler listarHandler,
            IDepositCommandHandler depositHandler,
            IListarTransaccionesQueryHandler transaccionesHandler)
        {
            _listarHandler = listarHandler;
            _depositHandler = depositHandler;
            _transaccionesHandler = transaccionesHandler;
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
            var resultado = await _depositHandler.Handle(command);
            return StatusCode(201, new { mensaje = "Depósito realizado con éxito", resultado });
        }

        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<TransaccionLedgerDto>>> GetTransactions(int id)
        {
            var query = new ListarTransaccionesQuery { Id  = id };
            var result = await _transaccionesHandler.Handle(query);
            return Ok(result);
        }
    }
}