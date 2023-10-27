using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Subject
    {
        [Key]
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }

        public int TeachingHours { get; set; }

        // Navigation properties
        public ICollection<SubjectAssign> SubjectAssigns { get; set; }
        public ICollection<ProfessorAssign> ProfessorAssigns { get; set; }

        public ICollection<Lecture> Lectures { get; set; }
    }
}
