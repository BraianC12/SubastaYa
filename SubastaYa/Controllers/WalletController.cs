using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.UseCases.Billeteras.Commands;
using Application.UseCases.Billeteras.Queries;
using Application.DTOs;


namespace SubastaYa.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;

        
        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{usuarioId}/balance")]
        public async Task<ActionResult<WalletBalanceDto>> GetBalance(int usuarioId)
        {
            
            var query = new ListarBilleteraQuery { Usuario_Id = usuarioId };
            var resultado = await _mediator.Send(query);

            return Ok(resultado);
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