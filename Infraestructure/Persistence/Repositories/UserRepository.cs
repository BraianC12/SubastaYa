using Application.Interfaces;
using Domain;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SubastaDbContext _context;

        public UserRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> Listar()
        {
            var users = await _context.Usuarios.ToListAsync();

            return users;
        }

        public async Task<Usuario> GetUser(string email)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }
    }
}
