using System.ComponentModel.DataAnnotations;

namespace AtonUserService.Dtos.Users
{
    public class LoginDto
    {
        [Required]
        public string Login { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
