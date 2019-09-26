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
        public DbSet<RaceDistance> Distances { get; set; }
        public DbSet<UserRace> UserRaces { get; set; }
        public DbSet<IdentificationNumber> IdentificationNumbers { get; set; }

        public TeamManagerDbContext(DbContextOptions<TeamManagerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("TeamManager");

            builder.Entity<RaceDistance>()
                .HasOne(p => p.Race)
                .WithMany()
                .HasForeignKey(p => p.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserRace>()
                .HasOne(ur => ur.Race)
                .WithMany()
                .HasForeignKey(ur => ur.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserRace>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IdentificationNumber>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}
