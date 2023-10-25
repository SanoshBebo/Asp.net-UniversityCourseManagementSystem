using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UCMS.Models;
using UCMS.Models.Domain;

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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                            return new Course
                            {
                                CourseId = Guid.Parse(reader["CourseId"].ToString()),
                                CourseName = reader["CourseName"].ToString(),
                                CourseDurationInYears = (int)reader["CourseDurationInYears"]
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
                string query = "INSERT INTO Courses (CourseId, CourseName, CourseDurationInYears) " +
                               "VALUES (@CourseId, @CourseName, @CourseDurationInYears)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", course.CourseId);
                    command.Parameters.AddWithValue("@CourseName", course.CourseName);
                    command.Parameters.AddWithValue("@CourseDurationInYears", course.CourseDurationInYears);
                    command.ExecuteNonQuery();
                }
            }   
        }

        private void UpdateCourse(Course course)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE Courses SET CourseName = @CourseName, CourseDurationInYears = @CourseDurationInYears " +
                               "WHERE CourseId = @CourseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", course.CourseId);
                    command.Parameters.AddWithValue("@CourseName", course.CourseName);
                    command.Parameters.AddWithValue("@CourseDurationInYears", course.CourseDurationInYears);
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
