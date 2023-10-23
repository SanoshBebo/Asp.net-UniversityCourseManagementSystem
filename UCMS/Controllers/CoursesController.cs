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
    public class CoursesController : Controller
    {
        private readonly UCMSDbContext _context;

        public CoursesController(UCMSDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
              return _context.Courses != null ? 
                          View(await _context.Courses.ToListAsync()) :
                          Problem("Entity set 'UCMSDbContext.Courses'  is null.");
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Courses == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            // Find the associated semesters
            var semesters = await _context.Semesters
                .Where(s => s.CourseId == id)
                .ToListAsync();

            var subjects = await _context.Subjects.ToListAsync();

            var courseWithSemesters = new Models.CourseWithSemesters
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseName,CourseDurationInYears")] Course course)
        {

            course.CourseId = Guid.NewGuid();
            _context.Add(course);

            for (int i = 1; i <= course.CourseDurationInYears; i++)
            {
                // Create two semesters for each year
                var semester1 = new Semester
                {
                    SemesterId = Guid.NewGuid(),
                    SemesterName = $"sem{i * 2 - 1}", // Odd semesters
                    CourseId = course.CourseId
                };
                _context.Semesters.Add(semester1);

                var semester2 = new Semester
                {
                    SemesterId = Guid.NewGuid(),
                    SemesterName = $"sem{i * 2}", // Even semesters
                    CourseId = course.CourseId
                };
                _context.Semesters.Add(semester2);
            }


            await _context.SaveChangesAsync();
             
            return RedirectToAction(nameof(Index));
                  
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseId,CourseName,CourseDurationInYears")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

       
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'UCMSDbContext.Courses'  is null.");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(Guid id)
        {
          return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}
