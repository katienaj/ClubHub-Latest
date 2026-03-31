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
    public class MeetingsController : Controller
    {
        private readonly DatabaseContext _context;

        public MeetingsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Meetings
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Meeting.Include(m => m.Club);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting
                .Include(m => m.Club)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meetings/Create
        public async Task<IActionResult> Create(int? clubId)
        {
            if (clubId == null)
            {
                return NotFound(); // Or handle as an error
            }

            var club = await _context.Clubs.FindAsync(clubId);
            if (club == null)
            {
                return NotFound();
            }

            var meeting = new Meeting
            {
                ClubId = club.ClubId,
                Club = club,
                MeetingDate = club.MeetingTime.HasValue ? DateTime.Today + club.MeetingTime.Value : DateTime.Today

            };

            return View(meeting);
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeetingId,ClubId,MeetingDate,Agenda")] Meeting meeting)
        {
            ModelState.Remove("Club");

            if (ModelState.IsValid)
            {
                _context.Add(meeting);
                await _context.SaveChangesAsync();

                // Create attendance records for all members of the club
                var members = await _context.Joins
                    .Where(j => j.ClubId == meeting.ClubId)
                    .ToListAsync();

                foreach (var member in members)
                {
                    var attendance = new Attendance
                    {
                        MeetingId = meeting.MeetingId,
                        PersonId = member.PersonId,
                        isPresent = false
                    };
                    _context.Attendance.Add(attendance);
                }
                await _context.SaveChangesAsync();

                // Redirect to the Attendance action after creating the meeting and attendance records
                return RedirectToAction("Attendance", new { id = meeting.MeetingId });
            }

            // If validation fails, we must reload the Club property
            // so the view can display the club name correctly.
            meeting.Club = await _context.Clubs.FindAsync(meeting.ClubId);
            if (meeting.Club == null)
            {
                // This is an edge case, but if the club is somehow deleted
                // between loading the page and submitting, handle it gracefully.
                return RedirectToAction("Index", "Clubs");
            }

            return View(meeting);
        }

        // GET: Meetings/Attendance/5
        public async Task<IActionResult> Attendance(int id)
        {
            var meetingWithAttendances = await _context.Meeting
                .Include(m => m.Club)
                .Include(m => m.Attendances)
                    .ThenInclude(a => a.Person)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meetingWithAttendances == null)
            {
                return NotFound();
            }

            return View(meetingWithAttendances);
        }

        // POST: Meetings/ToggleAttendance

        [HttpPost]
        public async Task<IActionResult> ToggleAttendance(int attendanceId, bool isPresent)
        {
            var attendance = await _context.Attendance.FindAsync(attendanceId);
            if (attendance == null)
            {
                return NotFound(new { success = false, message = "Attendance record not found." });
            }

            attendance.isPresent = isPresent;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isPresent = attendance.isPresent });
        }

        
       
        // POST: Meetings/DeleteMeeting
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeeting(int meetingId, string activeTab)
        {
            // Find the person's join object
            var findMeeting = await _context.Meeting
                .FirstOrDefaultAsync(j => j.MeetingId == meetingId );

            // If join object is found, delete the join
            if (findMeeting != null)
            {
                var clubId = findMeeting.ClubId;
                var attendances = _context.Attendance.Where(a => a.MeetingId == meetingId);
                _context.Attendance.RemoveRange(attendances);

                _context.Meeting.Remove(findMeeting);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Clubs", new { id = clubId, activeTab });
            }

            return NotFound();

        }
        

        // GET: Meetings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubId", meeting.ClubId);
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MeetingId,ClubId,MeetingDate,Agenda")] Meeting meeting)
        {
            if (id != meeting.MeetingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.MeetingId))
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
            ViewData["ClubId"] = new SelectList(_context.Clubs, "ClubId", "ClubId", meeting.ClubId);
            return View(meeting);
        }

        private bool MeetingExists(int id)
        {
            return _context.Meeting.Any(e => e.MeetingId == id);
        }
    }
}
