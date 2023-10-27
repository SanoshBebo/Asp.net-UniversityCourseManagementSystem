// UserController.cs
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UCMS.Data;
using UCMS.Models;
using UCMS.Models.Domain;

public class UsersController : Controller
{
    private readonly UCMSDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UsersController(UCMSDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = Guid.NewGuid();
            // Hash the password (you should use a password hashing library)
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create a new User entity
            var user = new User
            {
                UserId = userId,
                Name = model.Name,
                Email = model.Email,
                Password = hashedPassword,
                Role = model.Role
            };

            if (model.Role == "professor")
            {
                var professor = new Professor
                {
                    UserId = userId, // Ensure the same UserId
                    ProfessorName = model.Name,
                    ExperienceInYears = null,
                };
                _dbContext.Professors.Add(professor);
            }
            else if (model.Role == "student")
            {
                var student = new Student
                {
                    UserId = userId, // Ensure the same UserId
                    StudentName = model.Name,
                    Batch = null,
                    CourseId = null,
                    SemesterId = null,
                };
                _dbContext.Students.Add(student);
            }

            // Add the user to the database
            _dbContext.Users.Add(user);

            // Save changes to the database
            _dbContext.SaveChanges();

            // Redirect to a success page or login page
            return RedirectToAction("Login");
        }

        return View(model);
    }


    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                if (user.Role == "professor")
                {
                    return RedirectToAction("ProfessorHome", user);
                }
                else if (user.Role == "student")
                {
                    return RedirectToAction("StudentHome",user);
                }
                else if (user.Role == "admin")
                {
                    return RedirectToAction("AdminHome");
                }
            }
             ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    public IActionResult ProfessorHome(Guid userId)
    {
        // Retrieve the professor based on the user's ID
        var professor = _dbContext.Professors.FirstOrDefault(p => p.UserId == userId);

        if (professor != null)
        {
            // Use the professor object to fetch the assigned subjects with associated semester names
            var assignedSubjects = GetAssignedSubjectsForProfessor(professor.UserId);

            // Create a view model to pass the data to the view
            var professorHomeViewModel = new ProfessorHomeViewModel
            {
                Professor = professor,
                AssignedSubjects = assignedSubjects
            };

            return View(professorHomeViewModel);
        }

        // Handle the case where the professor is not found
        return View(); // You can display an error message or redirect to an error page.
    }

    private List<AssignedSubjectWithSemesterViewModel> GetAssignedSubjectsForProfessor(Guid professorId)
    {
        var assignedSubjects = _dbContext.ProfessorAssigns
            .Where(pa => pa.ProfessorId == professorId)
            .Join(_dbContext.Subjects,
                pa => pa.SubjectId,
                s => s.SubjectId,
                (pa, s) => new AssignedSubjectWithSemesterViewModel
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    SemesterId = pa.SemesterId,
                    CourseId = _dbContext.Semesters
                        .Where(semester => semester.SemesterId == pa.SemesterId)
                        .Select(semester => semester.CourseId)
                        .FirstOrDefault(),
                })
            .Join(_dbContext.Semesters,
                pa => pa.SemesterId,
                semester => semester.SemesterId,
                (pa, semester) => new AssignedSubjectWithSemesterViewModel
                {
                    SubjectId = pa.SubjectId,
                    SubjectName = pa.SubjectName,
                    SemesterId = pa.SemesterId,
                    CourseId = pa.CourseId,
                    SemesterName = semester.SemesterName,
                    CourseName = _dbContext.Courses
                        .Where(course => course.CourseId == pa.CourseId)
                        .Select(course => course.CourseName)
                        .FirstOrDefault()
                })
            .ToList();

        return assignedSubjects;
    }


    public IActionResult AssignLecture(Guid subjectId, Guid semesterId, Guid professorId)
    {
        var viewModel = new LectureAssignmentViewModel
        {
            SubjectId = subjectId,
            SemesterId = semesterId,
            UserId = professorId,
        };
        
        // Fetch the list of available venues from your database
        var venues = _dbContext.Venues.ToList();
        viewModel.Venues = venues;

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AddLecture(LectureAssignmentViewModel viewModel)
    {
        
            // Map the data from the view model to a new Lecture entity
            var newLecture = new Lecture
            {
                LectureId = Guid.NewGuid(),
                LectureName = viewModel.LectureName,
                UserId = viewModel.UserId,
                DateTime = viewModel.DateTime,
                Series = viewModel.Series,
                VenueId = viewModel.VenueId,
                SubjectId = viewModel.SubjectId,
                SemesterId = viewModel.SemesterId,
            };

            // Add the new lecture to the database
            _dbContext.Lectures.Add(newLecture);
            _dbContext.SaveChanges();

            // Redirect back to the professor's home page or any other appropriate page
            return RedirectToAction("ProfessorHome");
       
    }








    public IActionResult StudentHome(User user)
    {
        return View();
    }

    public IActionResult AdminHome()
    {
        return View();
    }
}

