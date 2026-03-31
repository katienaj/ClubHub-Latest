using System.ComponentModel.DataAnnotations;

namespace CSHonorsTemplate.Models
{
    public class Meeting
    {
        public int MeetingId { get; set; }
        public int ClubId { get; set; }
        public DateTime MeetingDate { get; set; }
        [StringLength(1500)]

        public string? Agenda { get; set; }
        public Club? Club { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    }
}
