using CSHonorsTemplate.Models;
using CSHonorsTemplate.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSHonorsTemplate.Controllers
{
    public class ClubsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IMiscHelper _miscHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClubsController(DatabaseContext context, IMiscHelper miscHelper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _miscHelper = miscHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Clubs
        public async Task<IActionResult> Index(string clubTypeFilter, string meetingDayFilter, string searchString)
        {
            // Base query to get clubs with their type
            var clubsQuery = _context.Clubs.Include(c => c.ClubType).AsQueryable();

            // Apply search filter for club name
            if (!string.IsNullOrEmpty(searchString))
            {
                clubsQuery = clubsQuery.Where(c => c.Name.Contains(searchString));
            }

            // Apply filter for club type
            if (!string.IsNullOrEmpty(clubTypeFilter))
            {
                if (int.TryParse(clubTypeFilter, out int clubTypeId))
                {
                    clubsQuery = clubsQuery.Where(c => c.ClubTypeId == clubTypeId);
                }
            }

            // Apply filter for meeting day
            if (!string.IsNullOrEmpty(meetingDayFilter))
            {
                if (Enum.TryParse<Days>(meetingDayFilter, out var day))
                {
                    clubsQuery = clubsQuery.Where(c => c.MeetingDay == day);
                }
            }

            // Populate ViewBag for filter dropdowns and user info
            UserInfo user = _miscHelper.GetUserInfo();
            ViewBag.isSiteAdmin = user.isAdmin;

            var clubTypes = await _context.ClubType.ToListAsync();
            ViewBag.ClubTypes = new SelectList(clubTypes, "ClubTypeId", "Name", clubTypeFilter);

            var meetingDays = Enum.GetNames(typeof(Days)).ToList();
            ViewBag.MeetingDays = new SelectList(meetingDays, meetingDayFilter);

            // Store current search string to repopulate the search box
            ViewData["CurrentFilter"] = searchString;

            // Execute the query and pass the results to the view
            var clubs = await clubsQuery.ToListAsync();
            return View(clubs);
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id, string activeTab = "info")
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get current user's PersonId
            UserInfo user = _miscHelper.GetUserInfo();
            var currentPersonId = user.person.PersonId;

            // Get and save if user is site admin
            var isSiteAdmin = user.isAdmin;

            // Get club with joins and people
            var club = await _context.Clubs
                .Include(c => c.ClubType)
                .Include(c => c.Meetings)
                .Include(c => c.Joins)
                    .ThenInclude(j => j.Person)
                .FirstOrDefaultAsync(m => m.ClubId == id);

            if (club == null)
            {
                return NotFound();
            }

            if (!club.isActive && !user.isAdmin)
            {
                return NotFound();
            }

            var clubId = club.ClubId;

            bool hasJoined = club.Joins != null && club.Joins.Any(j => j.PersonId == currentPersonId);

            // If user has joined, check if user is club admin
            bool isClubAdmin = false;
            if (hasJoined)
            {
                var existingJoin = await _context.Joins
                    .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == currentPersonId);
                if (existingJoin != null && (existingJoin.Role == Role.Advisor || existingJoin.Role == Role.Leader))
                {
                    isClubAdmin = true;
                }
            }

            // Get list of all Person IDs that are not in the club
            var nonClubMembers = await _context.People
                .Where(p => !p.Joins.Any(j => j.ClubId == id))
                .ToListAsync();

            // Get list of all club Members
            var clubMembers = await _context.People
                .Where(p => p.Joins.Any(j => j.ClubId == id && j.Role == Role.Member))
                .ToListAsync();

            // Get list of employees
            var employees = await _context.People
              .Where(p => p.PersonType == "Employee")
              .ToListAsync();

            var viewModel = new ClubDetailViewModel
            {
                Club = club,
                IsSiteAdmin = isSiteAdmin,
                IsClubAdmin = isClubAdmin,
                HasJoined = hasJoined,
                CurrentPersonId = currentPersonId,
                NonClubMembers = new SelectList(nonClubMembers, "PersonId", "FullName"),
                ClubMembers = new SelectList(clubMembers, "PersonId", "FullName"),
                Employees = new SelectList(employees, "PersonId", "FullName")

                
            };
            ViewBag.ActiveTab = activeTab;
            return View(viewModel);
        }


        // POST: Clubs/JoinClub
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinClub(int clubId, int personId, string activeTab)
        {
            
            // Check if this person is already a member of the club
            var existingJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == personId);

            if (existingJoin != null)
            {
                TempData["Message"] = "You are already registered with this club.";
                return RedirectToAction(nameof(Details), new { id = clubId });
            }

            var join = new Join { ClubId = clubId, PersonId = personId };

            _context.Add(join);
            await _context.SaveChangesAsync();
            TempData["Success"] = "You have successfully joined this club!";
            return RedirectToAction(nameof(Details),new {id= clubId, activeTab });
        }

        // POST: Clubs/ApproveMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMember(int clubId, int personId, string activeTab)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin =  await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == user.person.PersonId);
            if (user.isAdmin || (userJoin != null && (userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)))
            {
                // Find the person's join object
                var findJoin = await _context.Joins
                    .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == personId);

                // If join object is found, change role to Member
                if (findJoin != null)
                {
                    findJoin.Role = Role.Member;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = clubId, activeTab });
                }
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/RemoveUserFromClub
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromClub(int clubId, int personId, string activeTab)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == user.person.PersonId);
            if (user.isAdmin || (userJoin != null && (userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)))
            {
                // Find the person's join object
                var findJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == personId);

                // If join object is found, delete the join
                if (findJoin != null)
                {
                    _context.Joins.Remove(findJoin);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = clubId, activeTab });
                }
            }
            return View("~/Views/Error/PermissionDenied.cshtml");

        }

        // POST: Clubs/AddLeader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeader(int clubId, int personId, string activeTab)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == user.person.PersonId);
            if (user.isAdmin || (userJoin != null && (userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)))
            {
                // Find the person's join object
                var findJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == personId);

                // If join object is found, change role to Leader
                if (findJoin != null)
                {
                    findJoin.Role = Role.Leader;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = clubId, activeTab });
                }
                else
                {
                    ViewData["Name"] = user.person.FullName;
                    ViewData["Role"] = "Leader";
                    ViewData["ClubID"] = clubId;
                    return View("~/Views/Error/RoleAssignFailed.cshtml");
                }
            }
            return View("~/Views/Error/PermissionDenied.cshtml");

        }

        // POST: Clubs/AddAdvisor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdvisor(int clubId, int personId, string activeTab)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == user.person.PersonId);

            if (user.isAdmin || userJoin.Role == Role.Advisor)
            {
                // Find the person's join object
                var findJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == personId);
                Debug.WriteLine($"JOIN OBJECT: {findJoin}");
                // If join object is found, change role to advisor
                if (findJoin != null)
                {
                    findJoin.Role = Role.Advisor;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = clubId, activeTab });
                } else
                {
                    ViewData["Name"] = user.person.FullName;
                    ViewData["Role"] = "Advisor";
                    ViewData["ClubID"] = clubId;
                    return View("~/Views/Error/RoleAssignFailed.cshtml");
                }
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/HideMemberList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HideMemberList(int clubId, string activeTab)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == user.person.PersonId);
            if (user.isAdmin || userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)
            {
                var club = await _context.Clubs
                    .FirstOrDefaultAsync(c => c.ClubId == clubId);
                club.ShowMembers = false;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = clubId, activeTab });
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/ShowMemberList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowMemberList(int clubId, string activeTab)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == clubId && j.PersonId == user.person.PersonId);
            if (user.isAdmin || userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)
            {
                var club = await _context.Clubs
                    .FirstOrDefaultAsync(c => c.ClubId == clubId);
                club.ShowMembers = true;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = clubId, activeTab });
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // GET: Clubs/Create
        public async Task<IActionResult> Create()
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                // Get list of employees
                var employees = await _context.People
              .Where(p => p.PersonType == "Employee")
              .ToListAsync();

                ViewBag.Employees = new SelectList(employees, "PersonId", "FullName");

                // Get list of club types
                var clubTypes = await _context.ClubType
                    .ToListAsync();

                ViewBag.ClubTypes = new SelectList(clubTypes, "ClubTypeId", "Name");

                return View();
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubId,VeracrossId,Name,ClubTypeId,MeetingDay,MeetingTime,MeetingFrequency,MeetingLocation,Description")] Club club, int SelectedSponsor, IFormFile? imageFile)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin )
            {
                if (imageFile != null)
                {
                    if (!imageFile.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "The uploaded file is not a valid image.");
                    }
                    else
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        club.Image = uniqueFileName;
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(club);
                    await _context.SaveChangesAsync();

                    // Create join for advisor

                    if (SelectedSponsor != 0)
                    {
                        var join = new Join { ClubId = club.ClubId, PersonId = SelectedSponsor, Role = Role.Advisor };

                        _context.Add(join);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }

                var employees = await _context.People
                  .Where(p => p.PersonType == "Employee")
                  .ToListAsync();

                ViewBag.Employees = new SelectList(employees, "PersonId", "FullName", SelectedSponsor);

                var clubTypes = await _context.ClubType
                    .ToListAsync();

                ViewBag.ClubTypes = new SelectList(clubTypes, "ClubTypeId", "Name", club.ClubType);

                return View(club);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // GET: Clubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == id && j.PersonId == user.person.PersonId);
            if (user.isAdmin || (userJoin != null && (userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var club = await _context.Clubs.FindAsync(id);
                if (club == null)
                {
                    return NotFound();
                }

                // Get list of club types
                var clubTypes = await _context.ClubType
                    .ToListAsync();

                ViewBag.ClubTypes = new SelectList(clubTypes, "ClubTypeId", "Name", club.ClubTypeId);

                return View(club);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClubId,VeracrossId,Name,ClubTypeId,MeetingDay,MeetingTime,MeetingFrequency,MeetingLocation,Description")] Club club, IFormFile? imageFile)

        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            var userJoin = await _context.Joins
                .FirstOrDefaultAsync(j => j.ClubId == id && j.PersonId == user.person.PersonId);
            if (user.isAdmin || (userJoin != null && (userJoin.Role == Role.Advisor || userJoin.Role == Role.Leader)))
            {
                if (id != club.ClubId)
                {
                    return NotFound();
                }

                if (imageFile != null)
                {
                    if (!imageFile.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("Image", "The uploaded file is not a valid image.");
                    }
                    else
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        club.Image = uniqueFileName;
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var clubToUpdate = await _context.Clubs.FindAsync(id);
                        if (clubToUpdate == null)
                        {
                            return NotFound();
                        }

                        clubToUpdate.VeracrossId = club.VeracrossId;
                        clubToUpdate.Name = club.Name;
                        clubToUpdate.ClubTypeId = club.ClubTypeId;
                        clubToUpdate.MeetingDay = club.MeetingDay;
                        clubToUpdate.MeetingTime = club.MeetingTime;
                        clubToUpdate.MeetingFrequency = club.MeetingFrequency;
                        clubToUpdate.MeetingLocation = club.MeetingLocation;
                        clubToUpdate.Description = club.Description;
                        if (imageFile != null)
                        {
                            clubToUpdate.Image = club.Image;
                        }

                        _context.Update(clubToUpdate);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ClubExists(club.ClubId))
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

                var clubTypes = await _context.ClubType
                    .ToListAsync();

                ViewBag.ClubTypes = new SelectList(clubTypes, "ClubTypeId", "Name", club.ClubTypeId);

                return View(club);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // GET: Clubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var club = await _context.Clubs
                    .FirstOrDefaultAsync(m => m.ClubId == id);
                if (club == null)
                {
                    return NotFound();
                }

                return View(club);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                var club = await _context.Clubs.FindAsync(id);

                if (club != null)
                {
                    _context.Clubs.Remove(club);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPage", "Home");
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // GET: Clubs/Disable/5
        public async Task<IActionResult> Disable(int? id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var club = await _context.Clubs
                    .FirstOrDefaultAsync(m => m.ClubId == id);
                if (club == null)
                {
                    return NotFound();
                }

                return View(club);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableConfirmed(int id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                var club = await _context.Clubs.FindAsync(id);

                if (club != null)
                {
                    club.isActive = false;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPage", "Home");
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // GET: Clubs/Enable/5
        public async Task<IActionResult> Enable(int? id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var club = await _context.Clubs
                    .FirstOrDefaultAsync(m => m.ClubId == id);
                if (club == null)
                {
                    return NotFound();
                }

                return View(club);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        // POST: Clubs/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable(int id)
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                var club = await _context.Clubs.FindAsync(id);

                if (club != null)
                {
                    club.isActive = true;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPage", "Home");
            }
            return View("~/Views/Error/PermissionDenied.cshtml");

        }

        private bool ClubExists(int id)
        {
            return _context.Clubs.Any(e => e.ClubId == id);
        }
    }
}
