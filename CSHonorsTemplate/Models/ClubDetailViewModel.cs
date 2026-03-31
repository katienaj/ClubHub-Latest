using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSHonorsTemplate.Models
{
    public class ClubDetailViewModel
    {
        public Club Club { get; set; }
        public UserInfo UserInfo { get; set; }
        public bool HasJoined { get; set; }
        public bool IsClubAdmin { get; set; } = false;
        public bool IsSiteAdmin { get; set; }
        public int CurrentPersonId { get; set; }
        public SelectList? ClubMembers { get; set; }
        public SelectList? NonClubMembers { get; set; }
        public SelectList? Employees { get; set; }

        //etc...fill in the rest of the things you pass to the view here
    }
}
