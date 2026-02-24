using System.ComponentModel.DataAnnotations;

namespace MiniDatingApp.Models
{
    public class Availability
    {
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }

        [Required]
        public DateTime? EndTime { get; set; }
    }
}
