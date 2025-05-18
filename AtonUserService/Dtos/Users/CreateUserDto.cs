using System.ComponentModel.DataAnnotations;

namespace AtonUserService.Dtos.Users
{
    public class CreateUserDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; } = "";
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; } = "";
        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$")]
        public string Name { get; set; } = "";
        [Required]
        [Range(0,2)]
        public int Gender { get; set; } = 2;
        public DateTime? Birthday { get; set; }
        [Required]
        public bool Admin { get; set; } = false;
    }
}
