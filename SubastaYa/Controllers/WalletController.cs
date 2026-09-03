using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.UseCases.Billeteras.Commands;

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