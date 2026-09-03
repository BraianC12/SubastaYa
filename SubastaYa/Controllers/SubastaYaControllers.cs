using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.UseCases.Subastas.Commands;

namespace SubastaYa.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class SubastasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubastasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubasta([FromBody] CreateSubastaCommand command)
        {
            int id = await _mediator.Send(command);

            return StatusCode(201, new { mensaje = "Subasta creada exitosamente", id });
        }

        [HttpPost("{id}/bids")]
        public async Task<IActionResult> CreateBid(int id, [FromBody] CreateBidCommand command)
        {
            command.Subasta_Id = id;

            int pujaId = await _mediator.Send(command);

            return StatusCode(201, new { mensaje = "Puja creada exitosamente", id = pujaId });
        }
    }
}