using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCMS.Models.Domain
{
    public class Semester
    {
        [Key]
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; }

        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        // Navigation properties
        public Course Course { get; set; }
        public ICollection<StudentRegistration> StudentRegistration { get; set; }
        public ICollection<SubjectAssign> SubjectAssigns { get; set; }
        public ICollection<ProfessorAssign> ProfessorAssigns { get; set; }
        public ICollection<Lecture> Lectures { get; set; }
    }
}
