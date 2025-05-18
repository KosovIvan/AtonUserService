using System.ComponentModel.DataAnnotations;

namespace AtonUserService.Dtos.Users
{
    public class UpdateDataUserDto
    {
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$")]
        public string? Name { get; set; }
        [Range(0, 2)]
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}