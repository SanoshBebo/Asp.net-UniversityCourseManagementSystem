namespace UCMS.Models
{
    public class AssignedSubjectWithSemesterViewModel
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
    }

}
