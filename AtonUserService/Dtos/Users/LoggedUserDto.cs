namespace AtonUserService.Dtos.Users
{
    public class LoggedUserDto
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; } = 2;
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Token { get; set; }
    }
}
