using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class StudentLectureEnrollment
    {
        [Key, Column(Order = 0)]
        public Guid StudentId { get; set; }

        [Key, Column(Order = 1)]
        public Guid LectureId { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Lecture Lecture { get; set; }
    }

}
