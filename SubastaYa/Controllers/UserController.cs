using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Application.UseCases.Usuarios.Queries;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGetUserQueryHandler _queryHandler;
        private readonly ICreateUserCommandHandler _commandHandler;

        public UserController(IGetUserQueryHandler queryHandler, ICreateUserCommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand request)
        {
            int userId = await _commandHandler.Handle(request);

            return StatusCode(201, new { mensaje = "Usuario creada exitosamente", id = userId });
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> Login([FromQuery]GetUserQuery query)
        {
            var user = await _queryHandler.Handle(query);

            return Ok(user);
        }
    }
}
