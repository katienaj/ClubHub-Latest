using Microsoft.IdentityModel.Logging;

namespace CSHonorsTemplate.Models
{
    public class UserInfo
    {
        public Person person { get; set; }
        public bool isAdmin { get; set; }
        public bool isStudent { get; set; }
        public bool isEmployee { get; set; }
    }
}
