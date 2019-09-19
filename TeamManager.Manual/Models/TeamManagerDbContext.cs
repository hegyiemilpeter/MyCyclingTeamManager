using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class TeamManagerDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public TeamManagerDbContext(DbContextOptions<TeamManagerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("TeamManager");

            base.OnModelCreating(builder);
        }
    }
}
