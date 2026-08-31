using Domain;
using Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubastaYa.DTOs;

namespace SubastaYa.Controllers
{
    
    [Route("api/auctions")]
    [ApiController]
    public class SubastasController : ControllerBase
    {
        private readonly SubastaDbContext _context;

        public SubastasController(SubastaDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllSubastas()
        {
            // Consultamos asíncronamente y transformamos la entidad al DTO
            var subastas = await _context.Subastas
                .Select(s => new AuctionDto
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    Url_Imagen = s.Url_Imagen,
                    Precio_Base = s.Precio_Base,
                    Incremento_Minimo = s.Incremento_Minimo,
                    Fecha_Inicio = s.Fecha_Inicio,
                    Fecha_Fin = s.Fecha_Fin,
                    Estado = s.Estado,
                    Categoria_Id = s.Categoria_Id,
                    Vendedor_Id = s.Vendedor_Id
                })
                .ToListAsync();

            // Retorna HTTP 200 OK con la lista limpia
            return Ok(subastas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubasta([FromBody] CreateAuctionDto dto)
        {
            
            var nuevaSubasta = new Subasta
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Url_Imagen = dto.Url_Imagen,
                Precio_Base = dto.Precio_Base,
                Incremento_Minimo = dto.Incremento_Minimo,
                Fecha_Inicio = dto.Fecha_Inicio,
                Fecha_Fin = dto.Fecha_Fin,
                Categoria_Id = dto.Categoria_Id,
                Vendedor_Id = dto.Vendedor_Id,
                Estado = "Activa" // Regla de negocio: toda subasta nueva nace como Activa
            };

            
            _context.Subastas.Add(nuevaSubasta);

            
            await _context.SaveChangesAsync();

            return StatusCode(201, new { mensaje = "Subasta creada exitosamente", id = nuevaSubasta.Id });

        }
    }
}