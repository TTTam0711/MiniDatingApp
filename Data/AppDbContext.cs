using Microsoft.EntityFrameworkCore;
using MiniDatingApp.Models;

namespace MiniDatingApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Match> Matches { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>()
                .HasIndex(p => p.Email)
                .IsUnique(); // tránh email trùng
        }
    }
}
