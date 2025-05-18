using AtonUserService.Models;

namespace AtonUserService.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Users user);
    }
}
