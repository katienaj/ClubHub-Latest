using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSHonorsTemplate.Models;

namespace CSHonorsTemplate.Controllers
{
    public class ClubTypesController : Controller
    {
        private readonly DatabaseContext _context;

        public ClubTypesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: ClubTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClubType.ToListAsync());
        }

        // GET: ClubTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubType = await _context.ClubType
                .FirstOrDefaultAsync(m => m.ClubTypeId == id);
            if (clubType == null)
            {
                return NotFound();
            }

            return View(clubType);
        }

        // GET: ClubTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClubTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubTypeId,Name")] ClubType clubType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clubType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clubType);
        }

        // GET: ClubTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubType = await _context.ClubType.FindAsync(id);
            if (clubType == null)
            {
                return NotFound();
            }
            return View(clubType);
        }

        // POST: ClubTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClubTypeId,Name")] ClubType clubType)
        {
            if (id != clubType.ClubTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clubType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubTypeExists(clubType.ClubTypeId))
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
            return View(clubType);
        }

        // GET: ClubTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubType = await _context.ClubType
                .FirstOrDefaultAsync(m => m.ClubTypeId == id);
            if (clubType == null)
            {
                return NotFound();
            }

            return View(clubType);
        }

        // POST: ClubTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clubType = await _context.ClubType.FindAsync(id);
            if (clubType == null)
            {
                return NotFound();
            }

            // Check if any clubs are using this club type
            var isUsed = await _context.Clubs.AnyAsync(c => c.ClubTypeId == id);
            if (isUsed)
            {
                // Add an error message and return to the delete view
                ViewData["ErrorMessage"] = $"The club type '{clubType.Name}' cannot be deleted because it is currently assigned to one or more clubs. Please reassign those clubs to a different type before deleting this one.";
                return View("Delete", clubType);
            }

            try
            {
                _context.ClubType.Remove(clubType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // This can catch other potential database errors on delete
                ViewData["ErrorMessage"] = "An unexpected error occurred while trying to delete the club type.";
                return View("Delete", clubType);
            }
        }

        private bool ClubTypeExists(int id)
        {
            return _context.ClubType.Any(e => e.ClubTypeId == id);
        }
    }
}
