using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Interfaces
{
    public interface IDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Player> Players { get; }
        DbSet<Team> Teams { get; }
        DbSet<Transfer> Transfers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
