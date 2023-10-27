using UCMS.Models.Domain;

namespace UCMS.Models
{
    public class ProfessorHomeViewModel
    {
            public Professor Professor { get; set; }
            public List<AssignedSubjectWithSemesterViewModel> AssignedSubjects { get; set; }
        
    }
}
