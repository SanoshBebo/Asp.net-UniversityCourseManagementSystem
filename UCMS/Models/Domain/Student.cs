using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Student
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string StudentName { get; set; }
        public string? Batch { get; set; }

        [ForeignKey("Course")]
        public Guid? CourseId { get; set; }

        [ForeignKey("Semester")]
        public Guid? SemesterId { get; set; }

        // Navigation property
        public User User { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public ICollection<SubjectAssign> SubjectAssigns { get; set; }
    }


}

