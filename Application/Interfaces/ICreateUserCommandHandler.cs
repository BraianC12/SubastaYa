using Application.DTOs;
using Application.UseCases.Usuarios.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICreateUserCommandHandler
    {
        Task<CreateUserDto> Handle(CreateUserCommand request);
    }
}
