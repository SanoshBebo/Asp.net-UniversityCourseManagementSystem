using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UCMS.Models;
using UCMS.Models.Domain;
using Microsoft.AspNetCore.Authorization;

namespace UCMS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IConfiguration _configuration;

        public CoursesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("UCMSConnectionString");
        }

        // GET: Courses

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            List<Course> courses = new List<Course>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Courses";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Course course = new Course
                            {
                                CourseId = Guid.Parse(reader["CourseId"].ToString()),
                                CourseName = reader["CourseName"].ToString(),
                                CourseDurationInYears = (int)reader["CourseDurationInYears"]
                            };
                            courses.Add(course);
                        }
                    }
                }
            }
            return View(courses);
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "admin")]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course course = GetCourseById(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            // Find the associated semesters and subjects
            var semesters = GetSemestersByCourseId(course.CourseId);
            var subjects = GetSubjects();
            var professors = GetProfessors();
            var courseWithSemesters = new CourseWithSemesters
            {
                Course = course,
                Semesters = semesters,
                Subjects = subjects
            };

            return View(courseWithSemesters);
        }

        // GET: Courses/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Create(Course course)
        {
            
                course.CourseId = Guid.NewGuid();
                InsertCourse(course);

                // Create semesters for the course
                for (int i = 1; i <= course.CourseDurationInYears; i++)
                {
                    for (int j = 1; j <= 2; j++) // Two semesters per year
                    {
                        var semester = new Semester
                        {
                            SemesterId = Guid.NewGuid(),
                            SemesterName = $"Semester {i * 2 - (2 - j)}", // Generate semester names
                            CourseId = course.CourseId
                        };
                        InsertSemester(semester);
                    }
                }

                return RedirectToAction(nameof(Index));
            
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "admin")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course course = GetCourseById(id.Value);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(Guid id, Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            
                UpdateCourse(course);
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "admin")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course course = GetCourseById(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            if (DeleteCourse(id))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Problem("Entity set 'UCMSDbContext.Courses' is null.");
            }
        }
        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult AddSemester([FromForm] Guid courseId, [FromForm] string semesterName)
        {
            

            using (var connection = new SqlConnection(_configuration.GetConnectionString("UCMSConnectionString")))
            {
                connection.Open();

                using (var command = new SqlCommand("INSERT INTO Semesters (SemesterId, SemesterName, CourseId) VALUES (@SemesterId, @SemesterName, @CourseId)", connection))
                {
                    command.Parameters.AddWithValue("@SemesterId", Guid.NewGuid());
                    command.Parameters.Add(new SqlParameter("@SemesterName", System.Data.SqlDbType.NVarChar, 4000)).Value = semesterName;
                    command.Parameters.AddWithValue("@CourseId", courseId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Details", new { id = courseId });
        }



        // Action for deleting a semester
        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult DeleteSemester(Guid courseId, Guid semesterId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("UCMSConnectionString")))
            {
                connection.Open();

                // Delete the semester from the database
                using (var command = new SqlCommand("DELETE FROM Semesters WHERE SemesterId = @SemesterId", connection))
                {
                    command.Parameters.AddWithValue("@SemesterId", semesterId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Details", new { id = courseId });
        }


        [Authorize(Roles = "admin")]

        public IActionResult GetAssignedSubjects(Guid semesterId)
        {
            // Define a list to store the assigned subjects
            List<Subject> assignedSubjects = new List<Subject>();
            var subjects = GetSubjects();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("UCMSConnectionString")))
            {
                connection.Open();

                // Create a SELECT query to retrieve assigned subjects for the specified semester
                string selectQuery = "SELECT s.SubjectId, s.SubjectName FROM SubjectAssigns sa JOIN Subjects s ON sa.SubjectId = s.SubjectId WHERE sa.SemesterId = @SemesterId";

                using (var command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@SemesterId", semesterId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create Subject objects from the retrieved data
                            var subject = new Subject
                            {
                                SubjectId = reader.GetGuid(reader.GetOrdinal("SubjectId")),
                                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName"))
                            };

                            assignedSubjects.Add(subject);
                        }
                    }
                }
            }

            var SubjectAssignmentModel = new SubjectAssignmentModel
            {
                SemesterId = semesterId,
                AssignedSubjects = assignedSubjects,
                AvailableSubjects = subjects,
            };

            // Pass the list of assigned subjects to the view
            return View(SubjectAssignmentModel);
        }












        [Authorize(Roles = "admin")]

        public IActionResult AssignProfessors(Guid subjectId, Guid semesterId)
        {
            // Get a list of professors (you can replace this with your logic)
            var professors = GetProfessors();

            // Get the list of professors assigned to this subject
            var assignedProfessors = GetAssignedProfessorsForSubject(subjectId, semesterId);

            // Create a model that includes available professors and assigned professors
            var professorAssignmentModel = new ProfessorAssignmentModel
            {
                SubjectId = subjectId,
                SemesterId = semesterId,
                AvailableProfessors = professors,
                AssignedProfessors = assignedProfessors,
            };

            return View(professorAssignmentModel);
        }



        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult AddProfessorToSubjects(Guid SubjectId, Guid SemesterId,  List<Guid> SelectedProfessorIds)
{
    if (SelectedProfessorIds != null && SelectedProfessorIds.Any())
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("UCMSConnectionString")))
        {
            connection.Open();

            // Create a parameterized INSERT query to add professors to the specified subject
            string insertQuery = "INSERT INTO ProfessorAssigns (ProfessorAssignId, SubjectId, ProfessorId, SemesterId) VALUES (@ProfessorAssignId, @SubjectId, @ProfessorId, @SemesterId)";

            foreach (var professorId in SelectedProfessorIds)
            {
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ProfessorAssignId", Guid.NewGuid());
                    command.Parameters.AddWithValue("@SubjectId", SubjectId);
                    command.Parameters.AddWithValue("@ProfessorId", professorId);
                    command.Parameters.AddWithValue("@SemesterId", SemesterId);


                            try
                            {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected <= 0)
                        {
                            // Handle the error (e.g., log an error, display an error message)
                            // You can choose to continue or exit the loop based on your error handling strategy.
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                        {
                            // Handle the error indicating duplicate professor assignment
                            ViewBag.DuplicateProfessorAssignmentMessage = "One or more selected professors are already assigned to this subject.";
                            // You can choose to continue or exit the loop based on your error handling strategy.
                        }
                    }
                }
            }
        }
    }

    // Redirect back to the AssignProfessors page with the same subject
    return RedirectToAction("AssignProfessors", new { SubjectId, SemesterId });
}

        [Authorize(Roles = "admin")]

        public List<Professor> GetAssignedProfessorsForSubject(Guid SubjectId, Guid SemesterId)
        {
            List<Professor> assignedProfessors = new List<Professor>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("UCMSConnectionString")))
            {
                connection.Open();

                // Create a SELECT query to retrieve professors assigned to the specified subject and semester
                string selectQuery = @"
            SELECT p.UserId, p.ProfessorName
            FROM ProfessorAssigns pa
            JOIN Professors p ON pa.ProfessorId = p.UserId
            WHERE pa.SubjectId = @SubjectId AND pa.SemesterId = @SemesterId";

                using (var command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@SubjectId", SubjectId);
                    command.Parameters.AddWithValue("@SemesterId", SemesterId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create Professor objects from the retrieved data
                            var professor = new Professor
                            {
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                                ProfessorName = reader.GetString(reader.GetOrdinal("ProfessorName"))
                            };

                            assignedProfessors.Add(professor);
                        }
                    }
                }
            }

            return assignedProfessors;
        }



        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult AddSubjectToSemester(Guid semesterId, List<Guid> selectedSubjectIds)
{
    if (selectedSubjectIds != null && selectedSubjectIds.Any())
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("UCMSConnectionString")))
        {
            connection.Open();

            // Create a parameterized INSERT query to add subjects to the specified semester
            string insertQuery = "INSERT INTO SubjectAssigns (SubjectAssignId, SemesterId, SubjectId) VALUES (@SubjectAssignId, @SemesterId, @SubjectId)";

            foreach (var subjectId in selectedSubjectIds)
            {
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@SubjectAssignId", Guid.NewGuid());
                    command.Parameters.AddWithValue("@SemesterId", semesterId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected <= 0)
                        {
                                    // Handle the error (e.g., log an error, display an error message)
                                    // You can choose to continue or exit the loop based on your error handling strategy.
                                }
                            }
                    catch (SqlException ex)
                    {
                        // Check if the error is due to a duplicate entry (unique constraint violation)
                            // Set a message in ViewBag to indicate the subject is already assigned
                            ViewBag.DuplicateSubjectMessage = "One or more selected subjects are already assigned to this semester.";
                            // You can choose to continue or exit the loop based on your error handling strategy.
                    }
                }
            }
        }
    }

    // Redirect to the assigned subjects page
    return RedirectToAction("GetAssignedSubjects", new { semesterId });
}


        private Course GetCourseById(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Courses WHERE CourseId = @CourseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int year = (int)reader["Year"];
                            string batch = reader["Batch"].ToString();
                            return new Course
                            {
                                CourseId = Guid.Parse(reader["CourseId"].ToString()),
                                CourseName = reader["CourseName"].ToString(),
                                CourseDurationInYears = (int)reader["CourseDurationInYears"],
                                Year = year,
                                Batch = batch
                            };
                        }
                    }
                }
            }
            return null;
        }



        private List<Semester> GetSemestersByCourseId(Guid courseId)
        {
            List<Semester> semesters = new List<Semester>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Semesters WHERE CourseId = @CourseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", courseId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Semester semester = new Semester
                            {
                                SemesterId = Guid.Parse(reader["SemesterId"].ToString()),
                                SemesterName = reader["SemesterName"].ToString(),
                                CourseId = Guid.Parse(reader["CourseId"].ToString())
                            };
                            semesters.Add(semester);
                        }
                    }
                }
            }
            return semesters;
        }

        private List<Subject> GetSubjects()
        {
            List<Subject> subjects = new List<Subject>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Subjects";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Subject subject = new Subject
                            {
                                SubjectId = Guid.Parse(reader["SubjectId"].ToString()),
                                SubjectName = reader["SubjectName"].ToString(),
                                TeachingHours = (int)reader["TeachingHours"]
                            };
                            subjects.Add(subject);
                        }
                    }
                }
            }
            return subjects;
        }


        private List<Professor> GetProfessors()
        {
            List<Professor> professors = new List<Professor>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Professors";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Professor professor = new Professor
                            {
                                UserId = Guid.Parse(reader["UserId"].ToString()),
                                ProfessorName = reader["ProfessorName"].ToString(),
                            };
                            professors.Add(professor);
                        }
                    }
                }
            }
            return professors;
        }


        private void InsertCourse(Course course)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO Courses (CourseId, CourseName, CourseDurationInYears, Batch, Year) " +
                               "VALUES (@CourseId, @CourseName, @CourseDurationInYears, @Batch, @Year)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", course.CourseId);
                    command.Parameters.AddWithValue("@CourseName", course.CourseName);
                    command.Parameters.AddWithValue("@CourseDurationInYears", course.CourseDurationInYears);
                    command.Parameters.AddWithValue("@Batch", course.Batch);
                    command.Parameters.AddWithValue("@Year", course.Year);

                    command.ExecuteNonQuery();
                }
            }   
        }
        private void UpdateCourse(Course course)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE Courses SET CourseName = @CourseName, CourseDurationInYears = @CourseDurationInYears, Batch = @Batch, Year = @Year " + 
                               "WHERE CourseId = @CourseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", course.CourseId);
                    command.Parameters.AddWithValue("@CourseName", course.CourseName);
                    command.Parameters.AddWithValue("@CourseDurationInYears", course.CourseDurationInYears);
                    command.Parameters.AddWithValue("@Batch", course.Batch);
                    command.Parameters.AddWithValue("@Year", course.Year);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void InsertSemester(Semester semester)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO Semesters (SemesterId, SemesterName, CourseId) " +
                               "VALUES (@SemesterId, @SemesterName, @CourseId)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SemesterId", semester.SemesterId);
                    command.Parameters.AddWithValue("@SemesterName", semester.SemesterName);
                    command.Parameters.AddWithValue("@CourseId", semester.CourseId);
                    command.ExecuteNonQuery();
                }
            }
        }
        private bool DeleteCourse(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Courses WHERE CourseId = @CourseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
