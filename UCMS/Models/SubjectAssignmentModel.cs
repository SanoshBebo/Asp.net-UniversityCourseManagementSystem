using UCMS.Models.Domain;

namespace UCMS.Models
{
    public class SubjectAssignmentModel
    {
        public Guid SemesterId { get; set; }
        public List<Subject> AssignedSubjects { get; set; }
        public List<Subject> AvailableSubjects { get; set; }
        public List<Guid> SelectedSubjectIds { get; set; }
    }
}
