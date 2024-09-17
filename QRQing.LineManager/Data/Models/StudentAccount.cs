using System.ComponentModel.DataAnnotations;

namespace QRQing.LineManager.Data.Models
{
    public class StudentAccount : UserAccount
    {
        
        public int Id { get; set; }
        public override string Role => "Student";

        public Student Student { get; set; }
        public int StudentId { get; set; }
    }
}
