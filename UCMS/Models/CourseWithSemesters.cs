using UCMS.Models.Domain;

namespace UCMS.Models
{
    public class CourseWithSemesters
    {
        public Course Course { get; set; }
        public List<Semester> Semesters { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Professor> Professors { get; set; }
        public Semester NewSemester { get; set; }
    }
}
