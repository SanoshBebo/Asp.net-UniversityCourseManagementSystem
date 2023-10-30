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

        // Navigation property
        public User User { get; set; }

        public ICollection<StudentLectureEnrollment> LectureEnrollments { get; set; }
    }


}

