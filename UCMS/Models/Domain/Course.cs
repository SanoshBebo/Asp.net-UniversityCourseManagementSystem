using System;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Course
    {
        [Key]
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string Batch { get; set; } // Add the Batch field
        public int CourseDurationInYears { get; set; }

        public int Year { get ; set; }

        // Navigation properties
        public ICollection<Semester> Semesters { get; set; }

        public ICollection<StudentRegistration> StudentRegistration { get; set; }


    }
}
