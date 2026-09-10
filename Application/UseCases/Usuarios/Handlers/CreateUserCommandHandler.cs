using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain;
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

        public async Task<int> Handle(CreateUserCommand request)
        {
            var user = new Usuario
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Password_Hash = request.Password,
                Fecha_Registro = DateTime.UtcNow
            };

            await _userRepository.Add(user);

            var wallet = new Billetera { Usuario = user };
            await _walletRepository.Add(wallet);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }
    }
}
