using Application.DTOs;
using Application.UsesCases.Handlers;
using Application.UsesCases.Subastas.Queries;
using Domain;
using Infraestructure.Data;
using Infraestructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.UseCases.Subastas.Commands;

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