namespace CSHonorsTemplate.Models
{ 
    public enum Role
    {
        Leader,
        Advisor,
        Member
    }

    public class Join
    {
        public int JoinId { get; set; }
        public int ClubId { get; set; }
        public int PersonId { get; set; }

    //When role is null, person is only interested while not yet being a member
        public Role? Role { get; set; }

        public Club Club { get; set; } = null!;
        public Person Person { get; set; } = null!;
    }
}

