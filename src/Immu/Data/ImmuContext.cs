using Immu.Models;
using Microsoft.EntityFrameworkCore;

namespace Immu.Data
{
    public class ImmuContext : DbContext
    {
        public ImmuContext(DbContextOptions<ImmuContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            builder.Entity<User>()
                .HasMany<UserChallenge>(u => u.Challenges)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserEmail);

            builder.Entity<Challenge>()
                .HasMany<UserChallenge>(c => c.UserChallenges)
                .WithOne(uc => uc.Challenge)
                .HasForeignKey(uc => uc.ChallengeId);
        }
    }
}
