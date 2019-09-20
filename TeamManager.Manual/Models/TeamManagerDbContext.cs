using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class TeamManagerDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<RaceDistances> Distances { get; set; }

        public TeamManagerDbContext(DbContextOptions<TeamManagerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("TeamManager");

            builder.Entity<RaceDistances>()
                .HasOne(p => p.Race)
                .WithMany(p => p.Distances)
                .HasForeignKey(p => p.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}
