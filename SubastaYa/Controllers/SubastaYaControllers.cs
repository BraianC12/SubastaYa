using Domain;
using Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<AuctionDetailDto>>> GetSubastaById(int id)
        {
            var subasta = await _context.Subastas.Where(s => s.Id == id).Select(s => new AuctionDetailDto
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
                Vendedor_Id = s.Vendedor_Id,
                Puja_Actual = s.Pujas.OrderByDescending(p => p.Monto).Select(p => (decimal?)p.Monto).FirstOrDefault()
            }).FirstOrDefaultAsync();

            if (subasta == null)
            {
                return NotFound(new { mensaje = "Subasta no encontrada" });
            }

            return Ok(subasta);
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

        [HttpPost("{id}/bids")]
        public async Task<IActionResult> CreateBid(int id, [FromBody] CreateBidDto dto)
        {
            var subasta = await _context.Subastas.Where(s  => s.Id == id).Include(s => s.Pujas).FirstOrDefaultAsync();

            if (subasta == null)
            {
                return NotFound( new {mensaje = "Subasta no encontrada"});
            }
            else
                if (subasta.Vendedor_Id == dto.Comprador_Id)
                {
                    return BadRequest(new { mensaje = "El vendedor no puede pujar en su propia subasta" });
                }
                else
                    if (subasta.Estado.ToUpper() != "ACTIVA")
                    {
                        return BadRequest(new { mensaje = "La subasta ya finalizo" });
                    }

            var billeteraUsuario = await _context.Billeteras.Where(b => b.Usuario_Id == dto.Comprador_Id).FirstOrDefaultAsync();

            if (billeteraUsuario == null)
            {
                return NotFound(new { mensaje = "Billetera no encontrada" });
            }
            else
                if (billeteraUsuario.Saldo_Disponible < dto.Monto)
                {
                    return BadRequest(new { mensaje = "El saldo disponible es menor al monto seleccionado" });
                }

            var pujaActual = subasta.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

            decimal montoMinimo;

            if (pujaActual == null)
            {
                montoMinimo = subasta.Precio_Base;
            }
            else
            {
                montoMinimo = pujaActual.Monto + subasta.Incremento_Minimo;
            }

            if (dto.Monto < montoMinimo)
            {
                return BadRequest(new { mensaje = $"El monto mínimo para pujar debe ser {montoMinimo}" });
            }

            billeteraUsuario.Saldo_Disponible -= dto.Monto;
            billeteraUsuario.Saldo_Retenido += dto.Monto;

            var transaccionRetencion = new Transaccion_Ledger
            {
                Tipo = "RETENCION",
                Monto = dto.Monto,
                Fecha = DateTime.UtcNow,
                Subasta_Id = subasta.Id,
                Billetera_Id = billeteraUsuario.Id
                
            };
            _context.Transacciones_Ledger.Add(transaccionRetencion);

            // Devolver en caso que exista el dinero retenido al usuario que pujo antes
            if (pujaActual != null)
            {
                var billeteraPujaAnterior = await _context.Billeteras.Where(b => b.Usuario_Id == pujaActual.Comprador_Id)
                    .FirstOrDefaultAsync();

                if (billeteraPujaAnterior != null)
                {
                    billeteraPujaAnterior.Saldo_Retenido -= pujaActual.Monto;
                    billeteraPujaAnterior.Saldo_Disponible += pujaActual.Monto;

                    var transaccionLiberacion = new Transaccion_Ledger
                    {
                        Tipo = "LIBERACION",
                        Monto = pujaActual.Monto,
                        Fecha = DateTime.UtcNow,
                        Subasta_Id = subasta.Id,
                        Billetera_Id = billeteraPujaAnterior.Id
                    };

                    _context.Transacciones_Ledger.Add(transaccionLiberacion);
                }
            }

            // Regla Anti-Sniping
            var tiempoRestante = subasta.Fecha_Fin - DateTime.UtcNow;

            if (tiempoRestante <= TimeSpan.FromMinutes(1))
            {
                subasta.Fecha_Fin = subasta.Fecha_Fin.AddMinutes(2);

                var auditoria = new Auditoria_Log
                {
                    Entidad = "SUBASTA",
                    Entidad_Id = subasta.Id,
                    Accion = "EXTENSION_TIEMPO",
                    Usuario_Id = dto.Comprador_Id,
                    Detalle_Json = $"Subasta extendida 2 minutos por anti-sniping\", \"nueva fecha de finalizacion:{subasta.Fecha_Fin:O}",
                    Fecha = DateTime.UtcNow
                };
                _context.Auditorias_Log.Add(auditoria);
            }


            var nuevaPuja = new Puja
            {
                Monto = dto.Monto,
                Fecha_Puja = DateTime.UtcNow,
                Comprador_Id = dto.Comprador_Id,
                Subasta_Id = id,
            };

            if(nuevaPuja.Monto < subasta.Incremento_Minimo)
            {
                return BadRequest( new {mensaje = "El monto ingresado es menor al incremento minimo de la subasta"});
            }


            _context.Pujas.Add(nuevaPuja);


            await _context.SaveChangesAsync();

            return StatusCode(201, new { mensaje = "Puja creada exitosamente", id = nuevaPuja.Id });

        }
    }
}