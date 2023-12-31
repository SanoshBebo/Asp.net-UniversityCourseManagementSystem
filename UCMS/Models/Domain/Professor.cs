﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Professor
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string ProfessorName { get; set; }
        public int? ExperienceInYears { get; set; }

        // Navigation property
        public User User { get; set; }
        public ICollection<ProfessorAssign> ProfessorAssigns { get; set; }
        public ICollection<Lecture> Lectures { get; set; }

        public ICollection<VenueBooking> VenueBookings { get; set; }
    }
}
