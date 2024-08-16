using Microsoft.EntityFrameworkCore;
using OrmMiniProject.Contexts;
using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Generic;
using OrmMiniProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task RegisterUserAsync(User user)
        {
            var existingEmail = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (existingEmail)
            {
                throw new InvalidOperationException("Email is already in use.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<string>> GetAllEmailsAsync()
        {
            return await _context.Users.Select(u => u.Email).ToListAsync();
        }
    }
}
