using Microsoft.AspNetCore.DataProtection;
using System.ComponentModel.DataAnnotations;

namespace QRQing.LineManager.Data.Models
{
    public class Student
    {
        

        [Required]
        public int? AdvisorId { get; set; }

        public Advisor? Advisor { get; set; }

        [Required]
        public DateTime TimeJoined { get; set; }

        [Required]
        public int QueuePosition { get; set; }

        [Timestamp]
        public byte[]? Timestamp { get; set; }

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

        [Required]
        public bool InLine { get; set; }

        public StudentAccount StudentAccount { get; set; }
    }
}
