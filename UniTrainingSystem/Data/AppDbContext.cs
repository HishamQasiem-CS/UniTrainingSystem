using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniTrainingSystem.Models;

namespace UniTrainingSystem.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
        }
        public DbSet<Company> Company { get; set; }

        public DbSet<TrainingType> TrainingTypes { get; set; }
        public DbSet<CompanyTraining> CompanyTrainings { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
