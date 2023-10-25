using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UCMS.Models.Domain;

namespace UCMS.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly IConfiguration _configuration;

        public SubjectsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            var DbConnectionString = _configuration.GetConnectionString("UCMSConnectionString");
            return DbConnectionString;
        }

        // GET: Subjects
        public IActionResult Index()
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
            return View(subjects);
        }

        // GET: Subjects/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subject subject = GetSubjectById(id.Value);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Subject subject)
        {
            
                subject.SubjectId = Guid.NewGuid();
                InsertSubject(subject);
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Subjects/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subject subject = GetSubjectById(id.Value);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return NotFound();
            }

            
                UpdateSubject(subject);
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Subjects/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subject subject = GetSubjectById(id.Value);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            DeleteSubject(id);
            return RedirectToAction(nameof(Index));
        }

        private Subject GetSubjectById(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Subjects WHERE SubjectId = @SubjectId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Subject
                            {
                                SubjectId = Guid.Parse(reader["SubjectId"].ToString()),
                                SubjectName = reader["SubjectName"].ToString(),
                                TeachingHours = (int)reader["TeachingHours"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void InsertSubject(Subject subject)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO Subjects (SubjectId, SubjectName, TeachingHours) " +
                               "VALUES (@SubjectId, @SubjectName, @TeachingHours)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectId", subject.SubjectId);
                    command.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                    command.Parameters.AddWithValue("@TeachingHours", subject.TeachingHours);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateSubject(Subject subject)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE Subjects SET SubjectName = @SubjectName, TeachingHours = @TeachingHours " +
                               "WHERE SubjectId = @SubjectId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectId", subject.SubjectId);
                    command.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                    command.Parameters.AddWithValue("@TeachingHours", subject.TeachingHours);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteSubject(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Subjects WHERE SubjectId = @SubjectId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectId", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
