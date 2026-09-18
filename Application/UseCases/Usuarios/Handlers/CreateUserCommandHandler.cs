using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using BCrypt.Net;
using Domain;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Usuarios.Handlers
{
    public class CreateUserCommandHandler : ICreateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IBilleteraRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUserRepository userRepository, IBilleteraRepository walletRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserDto> Handle(CreateUserCommand request)
        {
            var existingUser = await _userRepository.GetUser(request.Email);

            if (existingUser != null)
            {
                throw new ConflictException("El correo electrónico ya se encuentra registrado.");
            }

            var user = new Usuario
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Password_Hash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fecha_Registro = DateTime.Now
            };

            await _userRepository.Add(user);

            var wallet = new Billetera { Usuario = user };
            await _walletRepository.Add(wallet);

            await _unitOfWork.SaveChangesAsync();

            return new CreateUserDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                BilleteraId = wallet.Id,
                FechaRegistro = user.Fecha_Registro
            };
        }
    }
}
