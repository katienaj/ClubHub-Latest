using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSHonorsTemplate.Models
{
    public enum Days
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Other
    }

    public enum  Frequency
    {
        Weekly,
        Monthly,
        EveryOtherWeek,
        Other
    }
    public class Club
    {
        public int ClubId { get; set; }
        public string Image { get; set; } = "default-image.jpg";
        public int? VeracrossId { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        public int? ClubTypeId { get; set; }

        [Display(Name = "Meeting Day")]
        public Days? MeetingDay { get; set; }

        [Display(Name = "Meeting Time")]
        public TimeSpan? MeetingTime { get; set; }

        [Display(Name = "Meeting Frequency")]
        public Frequency? MeetingFrequency { get; set; }

        [Display(Name = "Meeting Location")]
        public string? MeetingLocation { get; set; }
        [StringLength(1500)]
        public string? Description { get; set; }
        public bool isActive { get; set; } = true;
        public bool ShowMembers { get; set; } = true;

        [Display(Name = "Type")]
        public ClubType? ClubType { get; set; } = null!;
        public ICollection<Join> Joins { get; set; } = new List<Join>();
        public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
    }
}
