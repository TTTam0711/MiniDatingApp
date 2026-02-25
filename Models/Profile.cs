using System.ComponentModel.DataAnnotations;

namespace MiniDatingApp.Models
{
    public class Profile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(18, 100)]
        public int Age { get; set; }

        [Required]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}
