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

        public async Task<Users?> DeleteUserByLogin(string login, string? deleterLogin)
        {
            var deleted = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (deleted == null) return null;

            deleted.ModifiedOn = DateTime.Now;
            deleted.ModifiedBy = deleterLogin;
            deleted.RevokedOn = DateTime.Now;
            deleted.RevokedBy = deleterLogin;

            await context.SaveChangesAsync();

            return deleted;
        }

        public async Task<IEnumerable<Users>> GetActiveUsers()
        {
            return await context.Users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn).ToListAsync();
        }

        public async Task<Users?> GetUserByLogin(string login)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<IEnumerable<Users>> GetUsersAboveAge(int age)
        {
            var dateTime = DateTime.Now.AddYears(-age);
            return await context.Users.Where(u => u.Birthday != null).Where(u => u.Birthday <= dateTime).ToListAsync();
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

        public async Task<Users?> UpdateData(string login, UpdateDataUserDto user, string? modifierLogin)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser == null) return null;

            if (user.Name != null) existingUser.Name = user.Name;
            if (user.Gender != null) existingUser.Gender = user.Gender.Value;
            if (user.Birthday != null) existingUser.Birthday = user.Birthday;
            existingUser.ModifiedOn = DateTime.Now;
            existingUser.ModifiedBy = modifierLogin;

            await context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<Users?> UpdateLogin(string login, string new_login, string? modifierLogin)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser == null) return null;

            existingUser.Login = new_login;
            existingUser.ModifiedOn = DateTime.Now;
            existingUser.ModifiedBy = modifierLogin;

            await context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<Users?> UpdatePassword(string login, string password, string? modifierLogin)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser == null) return null;

            existingUser.Password = password;
            existingUser.ModifiedOn = DateTime.Now;
            existingUser.ModifiedBy = modifierLogin;

            await context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<Users?> UpdateRecover(string login, string modifierLogin)
        {
            var recoveredUser = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (recoveredUser == null) return null;

            recoveredUser.ModifiedOn = DateTime.Now;
            recoveredUser.ModifiedBy = modifierLogin;
            recoveredUser.RevokedOn = null;
            recoveredUser.RevokedBy = "";

            await context.SaveChangesAsync();

            return recoveredUser;
        }
    }
}