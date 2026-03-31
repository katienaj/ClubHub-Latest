using CSHonorsTemplate.Models;
using CSHonorsTemplate.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSHonorsTemplate.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IMiscHelper _miscHelper;
        public SettingsController(DatabaseContext context, IMiscHelper miscHelper)
        {
            _context = context;
            _miscHelper = miscHelper;
        }
        public IActionResult Index()
        {
            var userInfo = _miscHelper.GetUserInfo();
            if (userInfo == null || !userInfo.isAdmin)
            {
                return RedirectToAction("PermissionDenied", "Error");
            }
            return View();
        }

        public IActionResult Impersonate() 
        {
            var userInfo = _miscHelper.GetUserInfo();
            if (userInfo == null || !userInfo.isAdmin)
            {
                return RedirectToAction("PermissionDenied", "Error");
            }
            return View(_context.People.OrderBy(p => p.LastName).ToList());
        }

        public ActionResult SetImpersonation(string PersonId)
        {
            _miscHelper.SetImpersonation(PersonId);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ClearImpersonation()
        {
            _miscHelper.ClearImpersonation();
            return RedirectToAction("Index", "Home");
        }

        // 1. List all Persons and UserPermissions
        public IActionResult ManageAdmins()
        {
            var userInfo = _miscHelper.GetUserInfo();
            if (userInfo == null || !userInfo.isAdmin)
            {
                return RedirectToAction("PermissionDenied", "Error");
            }
            var persons = _context.People.ToList();
            var userPermissions = _context.UserPermissions.ToList();
            ViewBag.UserPermissions = userPermissions;
            return View(persons);
        }

        // 2. Add a Person to UserPermission as Admin
        [HttpPost]
        public IActionResult AddAdmin(int campusId)
        {
            // Prevent duplicates
            if (!_context.UserPermissions.Any(up => up.PersonId == campusId))
            {
                var newAdmin = new UserPermission
                {
                    PersonId = campusId,
                    IsAdmin = true
                };
                _context.UserPermissions.Add(newAdmin);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageAdmins");
        }
        [HttpPost]
        public IActionResult RemoveAdmin(int campusId)
        {
            var admin = _context.UserPermissions.FirstOrDefault(up => up.PersonId == campusId && up.IsAdmin);
            if (admin != null)
            {
                _context.UserPermissions.Remove(admin);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageAdmins");
        }
    }
}
