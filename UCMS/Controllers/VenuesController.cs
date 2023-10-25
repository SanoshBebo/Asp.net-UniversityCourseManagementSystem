using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UCMS.Models.Domain;

namespace UCMS.Controllers
{
    public class VenuesController : Controller
    {
        private readonly IConfiguration _configuration;

        public VenuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("UCMSConnectionString");
        }

        // GET: Venues
        public IActionResult Index()
        {
            List<Venue> venues = new List<Venue>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Venues";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Venue venue = new Venue
                            {
                                VenueId = Guid.Parse(reader["VenueId"].ToString()),
                                VenueName = reader["VenueName"].ToString(),
                                VenueLocation = reader["VenueLocation"].ToString()
                            };
                            venues.Add(venue);
                        }
                    }
                }
            }
            return View(venues);
        }

        // GET: Venues/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Venue venue = GetVenueById(id.Value);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venue venue)
        {
            
                venue.VenueId = Guid.NewGuid();
                InsertVenue(venue);
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Venues/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Venue venue = GetVenueById(id.Value);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            
                UpdateVenue(venue);
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Venues/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Venue venue = GetVenueById(id.Value);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            if (DeleteVenue(id))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Problem("Entity set 'UCMSDbContext.Venues' is null.");
            }
        }

        private Venue GetVenueById(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Venues WHERE VenueId = @VenueId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VenueId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Venue
                            {
                                VenueId = Guid.Parse(reader["VenueId"].ToString()),
                                VenueName = reader["VenueName"].ToString(),
                                VenueLocation = reader["VenueLocation"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void InsertVenue(Venue venue)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO Venues (VenueId, VenueName, VenueLocation) " +
                               "VALUES (@VenueId, @VenueName, @VenueLocation)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VenueId", venue.VenueId);
                    command.Parameters.AddWithValue("@VenueName", venue.VenueName);
                    command.Parameters.AddWithValue("@VenueLocation", venue.VenueLocation);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateVenue(Venue venue)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "UPDATE Venues SET VenueName = @VenueName, VenueLocation = @VenueLocation " +
                               "WHERE VenueId = @VenueId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VenueId", venue.VenueId);
                    command.Parameters.AddWithValue("@VenueName", venue.VenueName);
                    command.Parameters.AddWithValue("@VenueLocation", venue.VenueLocation);
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool DeleteVenue(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "DELETE FROM Venues WHERE VenueId = @VenueId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VenueId", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
