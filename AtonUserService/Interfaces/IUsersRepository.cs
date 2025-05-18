using AtonUserService.Dtos.Users;
using AtonUserService.Models;

namespace AtonUserService.Interfaces
{
    public interface IUsersRepository
    {
        Task<Users?> Login(LoginDto loginDto);

        Task Create(Users user);
        Task<bool> CheckLogin(string login);
        Task<Users?> UpdateData(string login, UpdateDataUserDto user, string? modifierLogin);
        Task<bool> IsRevoked(string login);
        Task<Users?> UpdatePassword(string login, string password, string? modifierLogin);
        Task<Users?> UpdateLogin(string login, string new_login, string? modifierLogin);
        Task<IEnumerable<Users>> GetActiveUsers();
        Task<Users?> GetUserByLogin(string login);
        Task<IEnumerable<Users>> GetUsersAboveAge(int age);
    }
}
