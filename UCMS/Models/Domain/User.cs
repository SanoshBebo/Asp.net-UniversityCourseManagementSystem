using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class User 
    {
        [Key]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        // Navigation properties
        public Professor Professor { get; set; }
        public Student Student { get; set; }

    }
}
