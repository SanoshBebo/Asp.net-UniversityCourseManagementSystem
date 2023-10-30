using UCMS.Models.Domain;

namespace UCMS.Models
{
    public class StudentHomeViewModel
    {
        public Guid UserId { get; set; }
        public bool HasRegisteredCourse { get; set; }
        public Course Course { get; set; }
        public Semester Semester { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Course> AvailableCourses { get; set; }
    }

}
