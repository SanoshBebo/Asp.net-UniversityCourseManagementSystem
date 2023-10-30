using System.ComponentModel.DataAnnotations;

namespace UCMS.Models.Domain
{
    public class Venue
    {
        [Key]
        public Guid VenueId { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }

        // Navigation property
        public ICollection<Lecture> Lectures { get; set; }

        public ICollection<VenueBooking> VenuesBooking { get; set;}
    }
}
