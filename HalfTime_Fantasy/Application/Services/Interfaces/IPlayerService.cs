using Contract.Requests;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<object> GetAsync(int playerId, int teamId);
        Task<Player> UpdateAsync(UpdatePlayerRequest player, int teamId);
        List<Player> CreatePlayers(Team team);
    }
}