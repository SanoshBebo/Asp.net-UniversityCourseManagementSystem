namespace UCMS.Models
{
    public class LectureViewModel
    {
        public Guid LectureId { get; set; }
        public string LectureName { get; set; }
        public DateTime DateTime { get; set; }
        public string SubjectName { get; set; }
        public string Series { get; set; }
        public string ProfessorName { get; set; }
        public string VenueName { get; set; }
        public string SemesterName { get; set; }

        public bool IsBooked { get; set; }

        public int? Slots { get; set; }

        public Guid UserId { get; set; }
    }

}
