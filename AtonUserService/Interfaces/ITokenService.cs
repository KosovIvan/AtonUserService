using AtonUserService.Models;

namespace AtonUserService.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(Users user);
    }
}
