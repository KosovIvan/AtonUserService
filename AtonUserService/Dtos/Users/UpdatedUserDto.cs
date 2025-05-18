namespace AtonUserService.Dtos.Users
{
    public class UpdatedUserDto
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; } = 2;
        public DateTime? Birthday { get; set; }
    }
}
