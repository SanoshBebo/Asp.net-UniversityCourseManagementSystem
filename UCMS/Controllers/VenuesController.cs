using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCMS.Data;
using UCMS.Models.Domain;

namespace UCMS.Controllers
{
    public class VenuesController : Controller
    {
        private readonly UCMSDbContext _context;

        public VenuesController(UCMSDbContext context)
        {
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
              return _context.Venues != null ? 
                          View(await _context.Venues.ToListAsync()) :
                          Problem("Entity set 'UCMSDbContext.Venues'  is null.");
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Venues == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,VenueLocation")] Venue venue)
        {
           
                venue.VenueId = Guid.NewGuid();
                _context.Venues.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Venues == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("VenueId,VenueName,VenueLocation")] Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Venues.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
           
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Venues == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Venues == null)
            {
                return Problem("Entity set 'UCMSDbContext.Venues'  is null.");
            }
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(Guid id)
        {
          return (_context.Venues?.Any(e => e.VenueId == id)).GetValueOrDefault();
        }
    }
}
