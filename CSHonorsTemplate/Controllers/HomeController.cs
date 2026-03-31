using CSHonorsTemplate.Models;
using CSHonorsTemplate.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CSHonorsTemplate.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMiscHelper _miscHelper;
        private readonly DatabaseContext _context;

        public HomeController(IMiscHelper miscHelper, DatabaseContext context)
        {
            _miscHelper = miscHelper;
            _context = context;

        }

        public IActionResult Index()
        {
            UserInfo user = _miscHelper.GetUserInfo();
            //ViewBag.IsAdmin = user.isAdmin;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult UserInfo()
        {
            UserInfo user = _miscHelper.GetUserInfo();
            return View(user);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> AdminPage()
        {
            // Get and save if user is site admin or club sponsor/leader
            UserInfo user = _miscHelper.GetUserInfo();
            if (user.isAdmin)
            {
                var viewModel = new AdminPageViewModel
                {
                    Clubs = await _context.Clubs.ToListAsync(),
                    ClubTypes = await _context.ClubType.ToListAsync()
                };
                return View(viewModel);
            }
            return View("~/Views/Error/PermissionDenied.cshtml");
        }
    }
}
