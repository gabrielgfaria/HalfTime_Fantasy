using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public partial class HalfTime_FantasyContext : DbContext, IDbContext
    {
        public HalfTime_FantasyContext()
        {
        }

        public HalfTime_FantasyContext(DbContextOptions<HalfTime_FantasyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MarketValue).HasColumnType("money");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Players_Teams");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.Budget).HasColumnType("money");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MarketValue).HasColumnType("money");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.HasIndex(e => e.PlayerId, "IX_Transfers")
                    .IsUnique();

                entity.Property(e => e.Value).HasColumnType("money");

                entity.HasOne(d => d.Player)
                    .WithOne(p => p.Transfer)
                    .HasForeignKey<Transfer>(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Transfers_Players");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.TeamId, "IX_Users")
                    .IsUnique();

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UserRole)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Team)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Users_Teams");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
