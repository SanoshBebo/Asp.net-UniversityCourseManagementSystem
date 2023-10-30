using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCMS.Models.Domain
{
    public class VenueBooking
    {
        [Key]
        public Guid BookingId { get; set; }

        public DateTime BookingStart { get; set; }

        public DateTime BookingEnd { get; set; }

        [ForeignKey("Professor")]
        public Guid ProfessorId { get; set; }

        [ForeignKey("Venue")]
        public Guid VenueId { get; set; }
        public Professor Professor { get; set; }
        public Venue Venue { get; set; }
    }
}

