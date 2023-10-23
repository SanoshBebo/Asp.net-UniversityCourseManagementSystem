using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Course
    {
        [Key]
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public int CourseDurationInYears { get; set; }

        // Navigation properties
        public ICollection<Semester> Semesters { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
