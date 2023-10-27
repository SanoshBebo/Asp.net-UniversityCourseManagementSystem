using UCMS.Models.Domain;

namespace UCMS.Models
{
    public class ProfessorAssignmentModel
    {
            public Guid SemesterId { get; set; }
        public Guid SubjectId { get; set; }
        public List<Professor> AssignedProfessors { get; set; }
            public List<Professor> AvailableProfessors { get; set; }
            public List<Guid> SelectedProfessorIds { get; set; }
    }
}
