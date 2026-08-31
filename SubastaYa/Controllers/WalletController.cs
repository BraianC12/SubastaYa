using Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubastaYa.DTOs;
using Domain;

namespace SubastaYa.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController: ControllerBase
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
    }
}
