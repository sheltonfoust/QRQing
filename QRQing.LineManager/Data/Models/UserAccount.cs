using Microsoft.AspNetCore.DataProtection;
using System.ComponentModel.DataAnnotations;

namespace QRQing.LineManager.Data.Models
{
    public abstract class UserAccount
    {
        [Required]
        public string? Email { get; set; }
        public abstract string Role { get; }
        [Required]
        public string? ConfirmationCode { get; set; }
        [Required]
        public DateTime? CodeSent { get; set; }
    }
}
