using System.ComponentModel.DataAnnotations;

namespace QRQing.LineManager.Data.Models
{
    public class Advisor
    {
        
        public List<Student> Students { get; set; } = new();
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string? Email { get; set; }

        public AdvisorAccount AdvisorAccount { get; set; }
    }
}
