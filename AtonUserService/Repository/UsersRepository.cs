using AtonUserService.Data;
using AtonUserService.Dtos.Users;
using AtonUserService.Interfaces;
using AtonUserService.Models;
using Microsoft.EntityFrameworkCore;

namespace AtonUserService.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext context;

        public UsersRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CheckLogin(string login)
        {
            if (await context.Users.AnyAsync(u => u.Login == login)) return false;
            return true;
        }

        public async Task Create(Users user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<Users?> Login(LoginDto loginDto)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Login == loginDto.Login && u.Password == loginDto.Password);
        }
    }
}