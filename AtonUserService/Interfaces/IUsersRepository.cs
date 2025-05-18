using AtonUserService.Dtos.Users;
using AtonUserService.Models;

namespace AtonUserService.Interfaces
{
    public interface IUsersRepository
    {
        Task<Users?> Login(LoginDto loginDto);

        Task Create(Users user);
        Task<bool> CheckLogin(string login);
        Task<Users?> UpdateData(string login, UpdateDataUserDto user);
        Task<bool> IsRevoked(string login);
        Task<Users?> UpdatePassword(string login, string password);
        Task<Users?> UpdateLogin(string login, string new_login);
    }
}
