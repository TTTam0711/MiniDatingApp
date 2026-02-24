namespace MiniDatingApp.Models
{
    public class Match
    {
        public int Id { get; set; }

        public Guid User1Id { get; set; }

        public Guid User2Id { get; set; }
    }
}
