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

        public async Task<bool> IsRevoked(string login)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if ((existingUser == null)|| (existingUser.RevokedOn != null)) return true;
            return false;
        }

        public async Task<Users?> Login(LoginDto loginDto)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Login == loginDto.Login && u.Password == loginDto.Password);
        }

        public async Task<Users?> UpdateData(string login, UpdateDataUserDto user)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser == null) return null;

            if (user.Name != null) existingUser.Name = user.Name;
            if (user.Gender != null) existingUser.Gender = user.Gender.Value;
            if (user.Birthday != null) existingUser.Birthday = user.Birthday;
            await context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<Users?> UpdateLogin(string login, string new_login)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser == null) return null;

            existingUser.Login = new_login;
            await context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<Users?> UpdatePassword(string login, string password)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser == null) return null;

            existingUser.Password = password;
            await context.SaveChangesAsync();

            return existingUser;
        }
    }
}