using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Queries;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Usuarios.Handlers
{
    public class GetUserQueryHandler : IGetUserQueryHandler
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserQuery request)
        {
            var user = await _userRepository.GetUser(request.Email);

            if(user == null)
            {
                throw new NotFoundException("Usuario no encontrado");
            }

            if(user.Password_Hash != request.Password)
            {
                throw new DomainException("Contraseña incorrecta");
            }

            var userDto = new UserDto()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Fecha_Registro = user.Fecha_Registro
            };

            return userDto;
        }
    }
}
