using System.ComponentModel.DataAnnotations;

namespace QRQing.LineManager.Data.Models
{
    public class AdvisorAccount : UserAccount
    {
        public int Id { get; set; }
        public override string Role => "Advisor";
        public Advisor Advisor { get; set; }
        public int AdvisorId { get; set; }
    }
}
