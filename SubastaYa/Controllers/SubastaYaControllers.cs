using Application.DTOs;
using Application.UsesCases.Handlers;
using Application.UsesCases.Subastas.Queries;
using Domain;
using Infraestructure.Data;
using Infraestructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SubastaYa.Controllers
{

    [Route("api/auctions")]
    [ApiController]
    public class SubastasController : ControllerBase
    {
        private readonly ObtenerSubastaQueryHandler _obtener;
        private readonly ListarSubastasQueryHandler _listar;

        public SubastasController(SubastaDbContext context)
        {
            _obtener = new ObtenerSubastaQueryHandler(new SubastaRepository(context));
            _listar = new ListarSubastasQueryHandler(new SubastaRepository(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllSubastas()
        {
            var subastas = await _listar.Handle();

            return Ok(subastas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDetailDto>> GetSubastaById(int id)
        {
            var subasta = await _obtener.Handle(new ObtenerSubastaQuery(id));

            return Ok(subasta);
        }
    }
}