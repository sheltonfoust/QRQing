using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRQing.LineManager.Data.Models;

namespace QRQing.LineManager.Data
{
    public class LineManagerDbContext : DbContext
    {
        public LineManagerDbContext(
            DbContextOptions<LineManagerDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Advisor> Advisors => Set<Advisor>();

        public DbSet<AdvisorAccount> AdvisorAccounts => Set<AdvisorAccount>();
        public DbSet<StudentAccount> StudentAccounts => Set<StudentAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Advisor>().HasData(
                new Advisor { Id = 1, FirstName = "Anna", LastName = "Rockstar", Email = "a.rockstar@ttu.edu" },
                new Advisor { Id = 2, FirstName = "Julia", LastName = "Developer", Email = "j.developer@ttu.edu" });

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "Thomas", LastName = "Huber", AdvisorId = 1, Email = "t.huber@ttu.edu", QueuePosition = 1, TimeJoined = DateTime.Now.AddMinutes(1), InLine = true },
                new Student { Id = 2, FirstName = "Sara", LastName = "Metroid", AdvisorId = 2, Email = "s.metroid@ttu.edu", QueuePosition = 2, TimeJoined = DateTime.Now.AddMinutes(2), InLine = true},
                new Student { Id = 3, FirstName = "Ben", LastName = "Rockstar", AdvisorId = 2, Email = "b.rockstar@ttu.edu" , QueuePosition = 3, TimeJoined = DateTime.Now.AddMinutes(3), InLine = true},
                new Student { Id = 4, FirstName = "Alex", LastName = "Rider", AdvisorId = 1, Email = "a.rider@ttu.edu" , QueuePosition = 4, TimeJoined = DateTime.Now.AddMinutes(4), InLine = true},
                new Student { Id = 5, FirstName = "Sophie", LastName = "Ramos", AdvisorId = 1, Email = "s.ramos@ttu.edu", QueuePosition = 5, TimeJoined = DateTime.Now.AddMinutes(5), InLine = true },
                new Student { Id = 6, FirstName = "Julien", LastName = "Russell", AdvisorId = 2, Email = "j.russel@ttu.edu" , QueuePosition = 6, TimeJoined = DateTime.Now.AddMinutes(6), InLine = true },
                new Student { Id = 7, FirstName = "Yvonne", LastName = "Snider", AdvisorId = 2, Email = "y.snider@ttu.edu" , QueuePosition = 7, TimeJoined = DateTime.Now.AddMinutes(7), InLine = true},
                new Student { Id = 8, FirstName = "Jasmin", LastName = "Curtis", AdvisorId = 1, Email = "j.curtis@ttu.edu" , QueuePosition = 8, TimeJoined = DateTime.Now.AddMinutes(8), InLine = true});

            modelBuilder.Entity<AdvisorAccount>().HasData(
                new AdvisorAccount { Id = 1, Email = "a.rockstar@ttu.edu", AdvisorId = 1, ConfirmationCode = "0", CodeSent = DateTime.Now},
                new AdvisorAccount { Id = 2, Email = "j.developer@ttu.edu", AdvisorId = 2, ConfirmationCode = "0", CodeSent = DateTime.Now });

            modelBuilder.Entity<StudentAccount>().HasData(
                new StudentAccount { Id = 3, Email = "t.huber@ttu.edu",StudentId = 1, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 4, Email = "s.metroid@ttu.edu",StudentId = 2, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 5, Email = "b.rockstar@ttu.edu",StudentId = 3, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 6, Email = "a.rider@ttu.edu",StudentId = 4, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 7, Email = "s.ramos@ttu.edu",StudentId = 5, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 8, Email = "j.russel@ttu.edu",StudentId = 6, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 9, Email = "y.snider@ttu.edu",StudentId = 7, ConfirmationCode = "0", CodeSent = DateTime.Now },
                new StudentAccount { Id = 10, Email = "j.curtis@ttu.edu",StudentId = 8, ConfirmationCode = "0", CodeSent = DateTime.Now });




        }
    }
}
