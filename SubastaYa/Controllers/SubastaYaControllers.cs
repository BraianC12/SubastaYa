using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.UseCases.Subastas.Commands;
using Application.UseCases.Subastas.Queries;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllSubastas()
        {
            // Usamos MediatR para la lectura
            var subastas = await _mediator.Send(new ListarSubastasQuery());
            return Ok(subastas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDetailDto>> GetSubastaById(int id)
        {
            // Usamos MediatR para la lectura
            var subasta = await _mediator.Send(new ObtenerSubastaQuery(id));
            return Ok(subasta);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubasta([FromBody] CreateSubastaCommand command)
        {
            int subastaId = await _mediator.Send(command);
            return StatusCode(201, new { mensaje = "Subasta creada exitosamente", id = subastaId });
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