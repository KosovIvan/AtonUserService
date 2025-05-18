using AtonUserService.Dtos.Users;
using AtonUserService.Models;

namespace AtonUserService.Interfaces
{
    public interface IUsersRepository
    {
        Task<Users?> Login(LoginDto loginDto);

        Task Create(Users user);
        Task<bool> CheckLogin(string login);
    }
}
