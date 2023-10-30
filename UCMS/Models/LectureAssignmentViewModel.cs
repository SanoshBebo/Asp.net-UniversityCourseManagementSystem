using UCMS.Models.Domain;

namespace UCMS.Models
{
    public class LectureAssignmentViewModel
    {
            
        public List<Venue> Venues{ get; set; }

        public Guid SubjectId { get; set; }
        public Guid ProfessorId { get; set; }
        public Guid SemesterId { get; set; }
        public string LectureName { get; set; }
        public DateTime DateTime { get; set; }
        public Guid VenueId { get; set; }
        public string Series {  get; set; }
        public List<Lecture> Lectures { get; set; }
        public List<string> LectureNames { get; set; }

        public LectureAssignmentViewModel()
        {
            LectureNames = new List<string>();
        }
    }

}
