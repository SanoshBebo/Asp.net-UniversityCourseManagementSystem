using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Professor
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string ProfessorName { get; set; }
        public int ExperienceInYears { get; set; }

        // Navigation property
        public User User { get; set; }
        public ICollection<SubjectAssign> SubjectAssigns { get; set; }
        public ICollection<Lecture> Lectures { get; set; }
    }
}
