namespace MiniDatingApp.Models
{
    public class Like
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
    }
}
