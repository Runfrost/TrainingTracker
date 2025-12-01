using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainingTrackerAPI.Models;

namespace TrainingTrackerAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.Gender)
            .HasConversion<string>();

            // Configure Table-Per-Hierarchy (TPH) inheritance
            modelBuilder.Entity<Activity>()
                .HasDiscriminator<string>("ActivityType")
                .HasValue<Running>("Running")
                .HasValue<Walking>("Walking")
                .HasValue<Cycling>("Cycling");

        }
    }
}
