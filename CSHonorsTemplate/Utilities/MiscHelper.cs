using Azure.Security.KeyVault.Certificates;
using CSHonorsTemplate.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace CSHonorsTemplate.Utilities
{
    public interface IMiscHelper
    {
        UserInfo GetUserInfo();
        bool isAdmin(int VCID);
        void SetImpersonation(string vercrossId);
        void ClearImpersonation();
    }

    public class MiscHelper : IMiscHelper
    {
        private readonly DatabaseContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MiscHelper(DatabaseContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public UserInfo GetUserInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Check if UserInfo is already cached for this request. If so, return it.
            if (httpContext.Items.TryGetValue("UserInfo", out var userInfoObj) && userInfoObj is UserInfo cachedUserInfo)
            {
                return cachedUserInfo;
            }

            //Pull in the user session information
            var session = _httpContextAccessor.HttpContext.Session;

            //Initialize variables used by the function
            var impersonatedUserId = "";
            string? userName = "";
            int ContextIdentityId = 0;
            var userInfo = new UserInfo
            {
                person = null,
                isAdmin = false,
                isStudent = false,
                isEmployee = false
            };
            //Check the session to see if we're impersonating
            impersonatedUserId = session.GetString("ImpersonatedUser");

            if (impersonatedUserId == "" || impersonatedUserId == null) //If we are not, pull in the user information
            {
                userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                var campusId = int.TryParse(userName?.Substring(0, 6), out var id) ? id : 0;
                userInfo.person = _db.People.FirstOrDefault(p => p.PersonId == campusId);
                userInfo.isAdmin = _db.UserPermissions.Any(up => up.PersonId == campusId && up.IsAdmin);
                userInfo.isStudent = userInfo.person?.PersonType == "Student";
                userInfo.isEmployee = userInfo.person?.PersonType == "Employee";
            }
            else //Else we are impersonating, so pull in that user information instead
            {
                var campusId = int.TryParse(impersonatedUserId.Substring(0, 6), out var id) ? id : 0;
                userInfo.person = _db.People.FirstOrDefault(p => p.PersonId == campusId);
                userInfo.isAdmin = _db.UserPermissions.Any(up => up.PersonId == campusId && up.IsAdmin);
                userInfo.isStudent = userInfo.person?.PersonType == "Student";
                userInfo.isEmployee = userInfo.person?.PersonType == "Employee";
            }

            // Cache the newly created UserInfo object for the duration of this request.
            if (userInfo != null)
            {
                httpContext.Items["UserInfo"] = userInfo;
            }

            return userInfo;
        }



        public bool isAdmin(int VCID)
        {
            var approver = _db.UserPermissions.FirstOrDefault(a => a.PersonId == VCID);
            if (approver != null)
            {
                return approver.IsAdmin;
            }
            return false;
        }

        public void SetImpersonation(string vercrossId)
        {
            _httpContextAccessor.HttpContext.Session.SetString("ImpersonatedUser", vercrossId);
        }

        public void ClearImpersonation()
        {
            _httpContextAccessor.HttpContext.Session.Remove("ImpersonatedUser");
        }

        public int GetIntFromString(string val)
        {
            int x = 0;

            if (Int32.TryParse(val, out x))
            {
                return x;
            }
            else
            {
                return x;
            }
        }
    }
}
