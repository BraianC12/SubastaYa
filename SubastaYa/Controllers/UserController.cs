using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGetUserQueryHandler _queryHandler;

        public UserController(IGetUserQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> Login([FromQuery]GetUserQuery query)
        {
            var user = await _queryHandler.Handle(query);

            return Ok(user);
        }
    }
}
