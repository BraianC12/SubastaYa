using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.UseCases.Subastas.Commands;
using Application.UseCases.Subastas.Queries;
using Application.UseCases.Subastas.Handlers;
using Application.UseCases.Handlers;
using Application.Interfaces;

namespace SubastaYa.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class SubastasController : ControllerBase
    {
        
        private readonly IListarSubastasQueryHandler _listarHandler;
        private readonly IObtenerSubastaQueryHandler _obtenerHandler;
        private readonly ICreateSubastaCommandHandler _createSubastaHandler;
        private readonly ICreateBidCommandHandler _createBidHandler;

        public SubastasController(
            IListarSubastasQueryHandler listarHandler, 
            IObtenerSubastaQueryHandler obtenerHandler,
            ICreateSubastaCommandHandler createSubastaHandler,
            ICreateBidCommandHandler createBidHandler)
        {
            _listarHandler = listarHandler;
            _obtenerHandler = obtenerHandler;
            _createSubastaHandler = createSubastaHandler;
            _createBidHandler = createBidHandler;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllSubastas([FromQuery]ListarSubastasQuery query)
        {
            var subastas = await _listarHandler.Handle(query);
            return Ok(subastas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDetailDto>> GetSubastaById(int id)
        {
            var subastas = await _obtenerHandler.Handle(new ObtenerSubastaQuery(id));
            return Ok(subastas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubasta(CreateSubastaCommand command)
        {
            int subastaId = await _createSubastaHandler.Handle(command);
            return StatusCode(201, new { mensaje = "Subasta creada exitosamente", id = subastaId });
        }

        [HttpPost("{id}/bids")]
        public async Task<IActionResult> CreateBid(int id, CreateBidCommand command)
        {
            command.Subasta_Id = id;
            int pujaId = await _createBidHandler.Handle(command);
            return StatusCode(201, new { mensaje = "Puja creada exitosamente", id = pujaId });
        }
    }
}