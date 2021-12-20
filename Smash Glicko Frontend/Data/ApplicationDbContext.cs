using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smash_Glicko_Frontend.Models;

namespace Smash_Glicko_Frontend.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<DatabaseLeagueModel> Leagues { get; set; }
        public DbSet<DatabaseEventModel> Events { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new LeagueEntityTypeConfiguration().Configure(modelBuilder.Entity<DatabaseLeagueModel>());
            new EventEntityTypeConfiguration().Configure(modelBuilder.Entity<DatabaseEventModel>());
            base.OnModelCreating(modelBuilder);
        }
    }
    public class LeagueEntityTypeConfiguration : IEntityTypeConfiguration<DatabaseLeagueModel>
    {
        public void Configure(EntityTypeBuilder<DatabaseLeagueModel> builder)
        {
            builder
                .Property(b => b.LeagueId)
                .IsRequired();
            builder
                .Property(b => b.LeagueName)
                .IsRequired();
            builder
                .Property(b => b.LeagueGame)
                .IsRequired();
            builder
                .Property(b => b.IsPublic)
                .IsRequired();
            builder
                .Property(b => b.InitalTimeFrameStart)
                .IsRequired();
            builder
                .Property(b => b.TimeFrameSpan)
                .IsRequired();
            builder.ToTable("LeaguesTable");
        }
    }
    public class EventEntityTypeConfiguration : IEntityTypeConfiguration<DatabaseEventModel>
    {
        public void Configure(EntityTypeBuilder<DatabaseEventModel> builder)
        {
            builder
                .Property(b => b.EventID)
                .IsRequired();
            builder
                .Property(b => b.EventSlug)
                .IsRequired();
            builder
                .Property(b => b.PlayerCount)
                .IsRequired();
            builder
                .Property(b => b.Player1ID)
                .IsRequired();
            builder
                .Property(b => b.Player2ID)
                .IsRequired();
            builder
                .Property(b => b.Player1Wins)
                .IsRequired();
            builder
                .Property(b => b.Player2Wins)
                .IsRequired();
            builder.ToTable("EventsTable");
        }
    }
}