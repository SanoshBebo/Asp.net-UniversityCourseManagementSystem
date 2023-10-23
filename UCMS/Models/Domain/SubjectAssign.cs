using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCMS.Models.Domain
{
    public class SubjectAssign
    {
        [Key]
        public Guid SubjectAssignId { get; set; }

        [ForeignKey("Semester")]
        public Guid SemesterId { get; set; }

        [ForeignKey("Professor")]
        public Guid ProfessorId { get; set; } // Explicitly specify the foreign key property name

        [ForeignKey("Subject")]
        public Guid SubjectId { get; set; }

        // Navigation properties
        public Semester Semester { get; set; }
        public Professor Professor { get; set; }
        public Subject Subject { get; set; }
    }
}
