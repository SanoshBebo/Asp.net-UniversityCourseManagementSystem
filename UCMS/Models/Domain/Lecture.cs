using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCMS.Models.Domain
{
    public class Lecture
    {
        [Key]
        public Guid LectureId { get; set; }
        public string LectureName { get; set; }

        [ForeignKey("Professor")]
        public Guid ProfessorId { get; set; } // Explicitly specify the foreign key property name

        public int Hours { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey("Venue")]
        public Guid VenueId { get; set; }

        [ForeignKey("Subject")]
        public Guid SubjectId { get; set; }

        [ForeignKey("Semester")]
        public Guid SemesterId { get; set; }

        // Navigation properties
        public Professor Professor { get; set; }
        public Venue Venue { get; set; }
        public Subject Subject { get; set; }
        public Semester Semester { get; set; }
    }

}
