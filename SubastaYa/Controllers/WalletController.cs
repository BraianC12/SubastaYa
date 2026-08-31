using Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Domain;

namespace SubastaYa.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly SubastaDbContext _context;

        public WalletController(SubastaDbContext context)
        {
            _context = context;
        }

        [HttpGet("balance")]
        public async Task<ActionResult<WalletBalanceDto>> GetBalance([FromQuery] int usuarioID)
        {
            var billetera = await _context.Billeteras
                .FirstOrDefaultAsync(b => b.Usuario_Id == usuarioID);

            if (billetera == null)
            {
                return NotFound(new { mensaje = "Billetera no encontrada para este usuario" });
            }

            var balance = new WalletBalanceDto
            {
                Saldo_Total = billetera.Saldo_Total,
                Saldo_Retenido = billetera.Saldo_Retenido,
                Saldo_Disponible = billetera.Saldo_Total - billetera.Saldo_Retenido
            };

            return Ok(balance);

        }

        //Endpoint POST: /api/wallet/deposit
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositoDto dto)
        {
            if (dto.Monto <= 0)
            {
                return BadRequest(new { mensaje = "El monto a depositar debe ser mayor a cero" });
            }

            var billetera = await _context.Billeteras
                .FirstOrDefaultAsync(b => b.Usuario_Id == dto.Usuario_Id);

            if (billetera == null)
            {
                return NotFound(new { mensaje = "Billetera no encontrada para el usuario" });
            }

            billetera.Saldo_Total += dto.Monto;

            await _context.SaveChangesAsync();

            // 6. Retornamos HTTP 200 OK confirmando la operación y mostrando cómo quedó la cuenta
            return Ok(new
            {
                mensaje = "Depósito acreditado exitosamente.",
                nuevoSaldoTotal = billetera.Saldo_Total
            });
        }
    }
}
