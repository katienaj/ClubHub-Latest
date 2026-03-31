namespace CSHonorsTemplate.Models
{
    public class ClubType
    {
        public int ClubTypeId { get; set; }

        public string? Name { get; set; }
        public ICollection<Club> Clubs { get; set; } = new List<Club>();

    }
}
