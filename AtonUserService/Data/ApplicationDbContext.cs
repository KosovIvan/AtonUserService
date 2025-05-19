using AtonUserService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtonUserService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            Users admin = new Users() { 
                Login = "Admin",
                Password = "admin1234",
                Name = "Admin", 
                Gender = 1,
                Birthday = new DateTime(2003, 11, 6),
                Admin = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "Иван",
                ModifiedOn = DateTime.Now,
                ModifiedBy = "Иван"
            };
            builder.Entity<Users>().HasData(admin);
        }
    }
}