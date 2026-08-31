using Infraestructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubastasController : ControllerBase
    {
        private readonly SubastaDbContext context;
        public SubastasController(SubastaDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAllSubastas()
        {
            var subastas = context.Subastas.ToList();
            return Ok(subastas);

        }

    }
}
