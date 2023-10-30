using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class StudentRegistration
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        [ForeignKey("Semester")]
        public Guid SemesterId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
    }
}
