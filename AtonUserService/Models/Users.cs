using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AtonUserService.Models
{
    public class Users
    {
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";
        public int Gender { get; set; } = 2;
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; } = "";
        public DateTime? RevokedOn { get; set; }
        public string RevokedBy { get; set; } = "";
    }
}