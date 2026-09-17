using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Queries;
using Application.UseCases.Handlers;
using Application.UseCases.Subastas.Commands;
using Application.UseCases.Subastas.Handlers;
using Application.UseCases.Subastas.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly IGetUserAuctionsQueryHandler _getUserAuctionsHandler;
        private readonly IGetBuyerBidsQueryHandler _getBuyerBidsHandler;

        public SubastasController(
            IListarSubastasQueryHandler listarHandler, 
            IObtenerSubastaQueryHandler obtenerHandler,
            ICreateSubastaCommandHandler createSubastaHandler,
            ICreateBidCommandHandler createBidHandler,
            IGetUserAuctionsQueryHandler getUserAuctionsHandler,
            IGetBuyerBidsQueryHandler getBuyerBidsHandler)
        {
            _listarHandler = listarHandler;
            _obtenerHandler = obtenerHandler;
            _createSubastaHandler = createSubastaHandler;
            _createBidHandler = createBidHandler;
            _getUserAuctionsHandler = getUserAuctionsHandler;
            _getBuyerBidsHandler = getBuyerBidsHandler;
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
            var result = await _createSubastaHandler.Handle(command);
            return StatusCode(201, new { mensaje = "Subasta creada exitosamente", result });
        }

        [HttpPost("{id}/bids")]
        public async Task<IActionResult> CreateBid(int id, CreateBidCommand command)
        {
            command.Subasta_Id = id;
            var result = await _createBidHandler.Handle(command);
            return StatusCode(201, new { mensaje = "Puja creada exitosamente", result });
        }

        [HttpGet("{vendedorId}/seller")]
        public async Task<ActionResult<List<SellerAuctionDto>>> GetSellerAuctions(int vendedorId)
        {
            var query = new GetUserAuctionsQuery { Id = vendedorId };
            var result = await _getUserAuctionsHandler.Handle(query);
            return Ok(result);
        }

        [HttpGet("{compradorId}/buyer")]
        public async Task<ActionResult<List<MyBidAuctionDto>>> GetBuyerBids(int compradorId)
        {
            var query = new GetBuyerBidsQuery { Id = compradorId };
            var result = await _getBuyerBidsHandler.Handle(query);
            return Ok(result);
        }

        [HttpGet("{id}/bids")]
        public async Task<IActionResult> GetHistorialPujas(int id, [FromServices] IGetHistorialPujasQueryHandler handler)
        {
            try
            {
                var query = new GetHistorialPujasQuery { SubastaId = id };
                var historial = await handler.Handle(query);

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el historial de pujas.", error = ex.Message });
            }
        }

    }
}