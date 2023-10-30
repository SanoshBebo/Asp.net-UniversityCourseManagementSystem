// UserController.cs
using System.Linq;
using Microsoft.AspNetCore.Identity;
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
    public async Task<IActionResult> Register(RegistrationViewModel model)
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
                };
                _dbContext.Students.Add(student);
            }

            // Add the user to the database
            _dbContext.Users.Add(user);

            // Save changes to the database
            _dbContext.SaveChanges();

            if (model.Role == "professor")
            {
                await userManager.AddToRoleAsync(user, "Professor");
            }
            else if (model.Role == "student")
            {
                await userManager.AddToRoleAsync(user, "Student");
            }
            else if (model.Role == "admin")
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }

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
            ProfessorId = professorId,
        };

        var subject = _dbContext.Subjects.FirstOrDefault(s => s.SubjectId == subjectId);
        var teachingHours = subject.TeachingHours;
        int numLectures = teachingHours / 2;

        // Generate a list of lecture names (e.g., L1, L2, L3, ...)
        viewModel.LectureNames = Enumerable.Range(1, numLectures).Select(i => $"L{i}").ToList();
        // Fetch the list of available venues from your database
        var venues = _dbContext.Venues.ToList();
        viewModel.Venues = venues;

        return View(viewModel);
    }

    public IActionResult ViewLecture(Guid subjectId, Guid semesterId, Guid professorId)
    {
        var viewModel = new LectureAssignmentViewModel
        {
            SubjectId = subjectId,
            SemesterId = semesterId,
            ProfessorId = professorId,
        };

        // Fetch the list of available venues from your database
        var venues = _dbContext.Venues.ToList();
        viewModel.Venues = venues;

        // Retrieve the lectures for the specified professor, subject, and semester
        var lectures = _dbContext.Lectures
            .Where(l => l.SubjectId == subjectId && l.SemesterId == semesterId && l.ProfessorId == professorId)
            .ToList();

        viewModel.Lectures = lectures;

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult DeleteLecture(Guid lectureId, Guid professorId)
    {
        // Find the lecture to delete
        var lecture = _dbContext.Lectures.FirstOrDefault(l => l.LectureId == lectureId);

        if (lecture != null)
        {
            // Delete the associated venue booking
            var venueBooking = _dbContext.VenueBookings.FirstOrDefault(vb => vb.BookingStart == lecture.DateTime && vb.VenueId == lecture.VenueId);

            if (venueBooking != null)
            {
                _dbContext.VenueBookings.Remove(venueBooking);
            }

            // Remove the lecture
            _dbContext.Lectures.Remove(lecture);
            _dbContext.SaveChanges();
        }

        // Redirect back to the "View Lectures" page
        return RedirectToAction("ViewLecture", new { subjectId = lecture.SubjectId, semesterId = lecture.SemesterId, professorId = professorId });
    }


    [HttpPost]
    public IActionResult AddLecture(LectureAssignmentViewModel viewModel)
    {
        // Retrieve the subject and semester to calculate remaining teaching hours
        var subject = _dbContext.Subjects.FirstOrDefault(s => s.SubjectId == viewModel.SubjectId);
        var semester = _dbContext.Semesters.FirstOrDefault(sm => sm.SemesterId == viewModel.SemesterId);

        if (subject != null && semester != null)
        {
            // Check if there is a lecture with the same name and series
            var existingLecture = _dbContext.Lectures.FirstOrDefault(l =>
                l.ProfessorId == viewModel.ProfessorId &&
                l.SubjectId == viewModel.SubjectId &&
                l.SemesterId == viewModel.SemesterId &&
                l.Series == viewModel.Series &&
                l.LectureName == viewModel.LectureName);

            if (existingLecture != null)
            {
                ModelState.AddModelError("LectureName", "A lecture with the same name and series already exists.");
            }
            else
            {
                // Query the database to get the count of Series 1 and Series 2 lectures for the professor, subject, and semester
                int series1LectureCount = _dbContext.Lectures.Count(l =>
                    l.ProfessorId == viewModel.ProfessorId &&
                    l.SubjectId == viewModel.SubjectId &&
                    l.SemesterId == viewModel.SemesterId &&
                    l.Series == "S1");

                int series2LectureCount = _dbContext.Lectures.Count(l =>
                    l.ProfessorId == viewModel.ProfessorId &&
                    l.SubjectId == viewModel.SubjectId &&
                    l.SemesterId == viewModel.SemesterId &&
                    l.Series == "S2");

                // Calculate the total teaching hours for Series 1 and Series 2 lectures
                int totalTeachingHoursSeries1 = series1LectureCount * 2;
                int totalTeachingHoursSeries2 = series2LectureCount * 2;

                // Calculate remaining teaching hours
                int remainingTeachingHours = (subject.TeachingHours * 2) - totalTeachingHoursSeries1 - totalTeachingHoursSeries2;

                // Check if there are enough teaching hours left to create a lecture
                if (remainingTeachingHours >= 2)
                {
                    // Check if the professor has exceeded the limit for either series
                    if ((viewModel.Series == "S1" && totalTeachingHoursSeries1 < 10) || (viewModel.Series == "S2" && totalTeachingHoursSeries2 < 10))
                    {
                        // Check if there is a previous lecture for the same series
                        var previousLecture = _dbContext.Lectures
                            .Where(l =>
                                l.ProfessorId == viewModel.ProfessorId &&
                                l.SubjectId == viewModel.SubjectId &&
                                l.SemesterId == viewModel.SemesterId &&
                                l.Series == viewModel.Series)
                            .OrderByDescending(l => l.DateTime)
                            .FirstOrDefault();

                        if (previousLecture != null)
                        {
                            // Calculate the time difference between the previous lecture and the new lecture
                            var timeDifference = viewModel.DateTime - previousLecture.DateTime;

                            // Ensure that the time difference is at least 1 day (24 hours)
                            if (timeDifference.TotalHours < 24)
                            {
                                ModelState.AddModelError("DateTime", "The next lecture for this series must be at least 1 day after the previous lecture.");
                            }
                        }

                        if (!ModelState.IsValid)
                        {
                            // There is no previous lecture for this series or the time difference is valid, proceed with venue availability check
                            // Check if the selected venue is available at the specified date and time
                            var isVenueAvailable = IsVenueAvailable(viewModel.VenueId, viewModel.DateTime, viewModel.DateTime.AddHours(2));

                            if (isVenueAvailable)
                            {
                                // Map the data from the view model to a new Lecture entity
                                var newLecture = new Lecture
                                {
                                    LectureId = Guid.NewGuid(),
                                    LectureName = viewModel.LectureName,
                                    DateTime = viewModel.DateTime,
                                    VenueId = viewModel.VenueId,
                                    SubjectId = viewModel.SubjectId,
                                    SemesterId = viewModel.SemesterId,
                                    ProfessorId = viewModel.ProfessorId,
                                    Series = viewModel.Series,
                                    StudentsEnrolled = 0,
                                };

                                // Add the new lecture to the database
                                _dbContext.Lectures.Add(newLecture);

                                // Insert the booking information into the VenueBookings table
                                var booking = new VenueBooking
                                {
                                    BookingId = Guid.NewGuid(),
                                    BookingStart = viewModel.DateTime,
                                    BookingEnd = viewModel.DateTime.AddHours(2), // Assuming 2 hours booking
                                    ProfessorId = viewModel.ProfessorId,
                                    VenueId = viewModel.VenueId
                                };

                                _dbContext.VenueBookings.Add(booking);

                                _dbContext.SaveChanges();

                                // Redirect back to the professor's home page or any other appropriate page
                                return RedirectToAction("ProfessorHome", new { userId = viewModel.ProfessorId });
                            }
                            else
                            {
                                ModelState.AddModelError("DateTime", "The selected venue is not available at the specified date and time.");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Series", "Maximum limit for lectures in this series reached.");
                    }
                }
                else
                {
                    ModelState.AddModelError("TeachingHours", "Not enough teaching hours left to create a lecture.");
                }
            }
        }
        else
        {
            // Handle the case where the subject or semester is not found
            if (subject == null)
            {
                ModelState.AddModelError("SubjectId", "Subject not found.");
            }
            if (semester == null)
            {
                ModelState.AddModelError("SemesterId", "Semester not found.");
            }
        }

        // Fetch the list of available venues from your database again
        var venues = _dbContext.Venues.ToList();
        viewModel.Venues = venues;

        var TeachingSubject = _dbContext.Subjects.FirstOrDefault(s => s.SubjectId == viewModel.SubjectId);
        var teachingHours = TeachingSubject.TeachingHours;
        int numLectures = teachingHours / 2;

        // Generate a list of lecture names (e.g., L1, L2, L3, ...)
        viewModel.LectureNames = Enumerable.Range(1, numLectures).Select(i => $"L{i}").ToList();

        // Pass the view model back to the view with errors
        return View("AssignLecture", viewModel);
    }



    // Define the IsVenueAvailable method in your controller
    private bool IsVenueAvailable(Guid venueId, DateTime bookingStart, DateTime bookingEnd)
    {
        // Query the VenueBookings table to check if the venue is available for the specified time period
        var existingBooking = _dbContext.VenueBookings.FirstOrDefault(booking =>
            booking.VenueId == venueId &&
            (booking.BookingStart <= bookingStart && booking.BookingEnd >= bookingStart) || // Check if bookingStart is within an existing booking
            (booking.BookingStart <= bookingEnd && booking.BookingEnd >= bookingEnd) ||     // Check if bookingEnd is within an existing booking
            (booking.BookingStart >= bookingStart && booking.BookingEnd <= bookingEnd)     // Check if the existing booking spans the entire period
        );

        return existingBooking == null;
    }

    [HttpPost]
    public IActionResult RegisterCourse(Guid courseId, Guid userId)
    {
        try
        {
            // Check if the user's record already exists in the StudentRegistrations table
            var existingRegistration = _dbContext.StudentRegistrations
                .FirstOrDefault(sr => sr.UserId == userId);

            if (existingRegistration != null)
            {
                // Student is already registered for a course
                // You can handle this case as needed (e.g., display an error message).
                return Content("You are already registered for a course.");
            }

            // Get the selected course
            var course = _dbContext.Courses.FirstOrDefault(c => c.CourseId == courseId);

            if (course != null)
            {
                // Determine the current semester based on the Batch and Year fields in the Course table
                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                int startYear = course.Year;

                int semesterOffset = (currentYear - startYear) * 2;

                if ((course.Batch == "January" && currentMonth > 6) || (course.Batch == "July" && currentMonth <= 6))
                {
                    semesterOffset += 1;
                }

                int currentSemester = semesterOffset + 1;

                // Determine the current semester name
                string currentSemesterName = "Semester " + currentSemester;

                // Find or create the corresponding Semester record in the database
                var semester = _dbContext.Semesters.FirstOrDefault(s => s.CourseId == courseId && s.SemesterName == currentSemesterName);



                // Create a new StudentRegistration record
                var studentRegistration = new StudentRegistration
                {
                    UserId = userId,
                    CourseId = courseId,
                    SemesterId = semester.SemesterId
                    // Set other properties if needed
                };

                _dbContext.StudentRegistrations.Add(studentRegistration);
                _dbContext.SaveChanges();

                // Redirect to the StudentHome page
                return RedirectToAction("StudentHome", new { userId });
            }

            // Handle the case where the selected course is not found
            return Content("The selected course does not exist.");
        }
        catch (Exception ex)
        {
            // Handle exceptions here, for example, log the exception and display an error view
            return Content("An error occurred: " + ex.Message); // You should create an Error.cshtml view
        }
    }
    public IActionResult StudentHome(Guid userId)
    {
        try
        {
            // Check if the user's record exists in the StudentRegistrations table
            var studentRegistration = _dbContext.StudentRegistrations
                .FirstOrDefault(sr => sr.UserId == userId);

            if (studentRegistration != null)
            {
                // Student has registered for a course, you can retrieve the course details
                var course = _dbContext.Courses.FirstOrDefault(c => c.CourseId == studentRegistration.CourseId);

                // You can also get the current semester if needed
                var semester = _dbContext.Semesters.FirstOrDefault(s => s.SemesterId == studentRegistration.SemesterId);

                // Query the SubjectAssign table to get subjects for the current semester
                var subjectAssignments = _dbContext.SubjectAssigns
                    .Where(sa => sa.SemesterId == studentRegistration.SemesterId)
                    .ToList();

                // Query the Subjects table to get subject details
                var subjects = subjectAssignments
                    .Select(sa => _dbContext.Subjects.FirstOrDefault(s => s.SubjectId == sa.SubjectId))
                    .ToList();

                // Pass the course, semester, and subjects to the view
                var model = new StudentHomeViewModel
                {
                    Course = course,
                    Semester = semester,
                    Subjects = subjects,
                    HasRegisteredCourse = true,
                    UserId = userId
                };

                return View(model);
            }

            // Student has not registered for a course, show all available courses
            var availableCourses = _dbContext.Courses.ToList();

            var availableCoursesModel = new StudentHomeViewModel
            {
                HasRegisteredCourse = false,
                AvailableCourses = availableCourses,
                UserId = userId,
            };

            return View(availableCoursesModel);
        }
        catch (Exception ex)
        {
            // Handle exceptions here, for example, log the exception and display an error view
            return Content("An error occurred: " + ex.Message); // You should create an Error.cshtml view
        }
    }



    public IActionResult ViewLectures(Guid userId,Guid subjectId, Guid semesterId)
    {
        var lectures = _dbContext.Lectures
            .Where(l => l.SubjectId == subjectId && l.SemesterId == semesterId)
            .ToList();

        var lectureViewModels = lectures.Select(l => new LectureViewModel
        {
            LectureId = l.LectureId,
            LectureName = l.LectureName,
            DateTime = l.DateTime,
            Series = l.Series,
            SubjectName = _dbContext.Subjects.FirstOrDefault(s => s.SubjectId == l.SubjectId)?.SubjectName,
            ProfessorName = _dbContext.Professors.FirstOrDefault(p => p.UserId == l.ProfessorId)?.ProfessorName,
            VenueName = _dbContext.Venues.FirstOrDefault(v => v.VenueId == l.VenueId)?.VenueName,
            SemesterName = _dbContext.Semesters.FirstOrDefault(s => s.SemesterId == l.SemesterId)?.SemesterName,
            IsBooked = IsLectureBooked(userId, l.LectureId),
            UserId = userId,
            Slots = l.StudentsEnrolled,
        })
        .ToList();

        return View(lectureViewModels);
    }

    private bool IsLectureBooked(Guid studentId, Guid lectureId)
    {
        return _dbContext.StudentLectureEnrollments
            .Any(enrollment => enrollment.StudentId == studentId && enrollment.LectureId == lectureId);
    }

    public IActionResult BookLecture(Guid lectureId, Guid userId)
    {
        var lecture = _dbContext.Lectures.FirstOrDefault(l => l.LectureId == lectureId);

        if (lecture == null)
        {
            TempData["ErrorMessage"] = "Invalid lecture.";
            return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
        }

        if (lecture.StudentsEnrolled >= 25)
        {
            TempData["ErrorMessage"] = "This lecture is fully booked.";
            return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
        }

        var subjectId = lecture.SubjectId;
        var lectureName = lecture.LectureName;

        var hasBookedSameNameLecture = _dbContext.StudentLectureEnrollments
            .Any(e => e.StudentId == userId && e.Lecture.SubjectId == subjectId && e.Lecture.LectureName == lectureName);

        if (hasBookedSameNameLecture)
        {
            TempData["ErrorMessage"] = $"You have already attended Lecture {lectureName} for this subject.";
            return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
        }

        int lectureNumber = int.Parse(lectureName.Substring(1));

        if (lectureNumber > 1)
        {
            var previousLectureName = "L" + (lectureNumber - 1);
            var hasBookedPreviousLecture = _dbContext.StudentLectureEnrollments
                .Any(e => e.StudentId == userId && e.Lecture.SubjectId == subjectId && e.Lecture.LectureName == previousLectureName);

            if (!hasBookedPreviousLecture)
            {
                TempData["ErrorMessage"] = $"You must complete Lecture {previousLectureName} before booking Lecture {lectureName}.";
                return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
            }
        }

        var existingEnrollment = _dbContext.StudentLectureEnrollments
            .FirstOrDefault(e => e.StudentId == userId && e.LectureId == lectureId);

        if (existingEnrollment != null)
        {
            TempData["ErrorMessage"] = "You have already booked this lecture.";
            return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
        }

        if(lecture.StudentsEnrolled < 25)
        {
        lecture.StudentsEnrolled++;

        }
        else
        {
            TempData["ErrorMessage"] = "Slot have been fully booked";
            return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
        }

        var enrollment = new StudentLectureEnrollment
        {
            StudentId = userId,
            LectureId = lectureId
        };

        _dbContext.StudentLectureEnrollments.Add(enrollment);
        _dbContext.SaveChanges();

        return RedirectToAction("ViewLectures", new { userId, lecture.SubjectId, lecture.SemesterId });
    }












    public IActionResult AdminHome()
    {
        return View();
    }
}

