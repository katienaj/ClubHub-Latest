namespace CSHonorsTemplate.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int MeetingId { get; set; }
        public int PersonId { get; set; }
        public bool isPresent { get; set; }
        public Person? Person { get; set; } = null!;

    }
}
