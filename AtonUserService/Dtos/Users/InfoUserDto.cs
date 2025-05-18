namespace AtonUserService.Dtos.Users
{
    public class InfoUserDto
    {
        public string Name { get; set; }
        public int Gender { get; set; } = 2;
        public DateTime? Birthday { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
