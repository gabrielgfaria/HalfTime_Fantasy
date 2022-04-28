using Application.Common;
using Application.Services.Interfaces;
using Contract.Requests;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PlayerService : IPlayerService
    {
        private const decimal PLAYER_VALUE = 1000000;

        private readonly IDbContext _context;
        private readonly IPlayerNameHandler _nameHandler;
        private readonly ICountryNameHandler _countryHandler;

        public PlayerService(IDbContext context,
            IPlayerNameHandler nameGenerator,
            ICountryNameHandler countryGenerator)
        {
            _context = context;
            _nameHandler = nameGenerator;
            _countryHandler = countryGenerator;
        }

        public async Task<object> GetAsync(int playerId, int teamId)
        {
            await ValidatePlayerAsync(playerId, teamId);
            return await _context.Players.Include(p => p.Team).FirstAsync(p => p.Id == playerId);
        }

        public async Task<Player> UpdateAsync(UpdatePlayerRequest updatePlayerRequest, int teamId)
        {
            await ValidatePlayerAsync(updatePlayerRequest.PlayerId, teamId);
            var playerToUpdate = ConstructPlayerToUpdate(updatePlayerRequest);
            await UpdateDatabaseAsync(playerToUpdate);

            return playerToUpdate;
        }

        public List<Player> CreatePlayers(Team team)
        {
            var players = new List<Player>();
            for (var i = 0; i < 20; i++)
            {
                var player = new Player();

                player.Position = AssignPlayerPosition(i);
                player.FirstName = _nameHandler.GenerateFirstName();
                player.LastName = _nameHandler.GenerateLastName();
                player.Country = _countryHandler.GenerateCountryName();
                player.MarketValue = PLAYER_VALUE;
                player.Age = (short)new Random().Next(18, 40);
                player.TeamId = team.Id;
                players.Add(player);
            }

            return players;
        }

        private static string AssignPlayerPosition(int i)
        {
            if (i < 3)
            {
                return PlayerPosition.Goalkeeper.ToString();
            }
            else if (i < 9)
            {
                return PlayerPosition.Defender.ToString();
            }
            else if (i < 15)
            {
                return PlayerPosition.Midfielder.ToString();
            }
            else
            {
                return PlayerPosition.Attacker.ToString();
            }
        }

        private async Task UpdateDatabaseAsync(Player playerToUpdate)
        {
            _context.Players.Update(playerToUpdate);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        private Player ConstructPlayerToUpdate(UpdatePlayerRequest updatePlayerRequest)
        {
            var playerToBeUpdated = _context.Players.FirstOrDefault(x => x.Id == updatePlayerRequest.PlayerId);
            playerToBeUpdated.FirstName = string.IsNullOrWhiteSpace(updatePlayerRequest.FirstName) ? playerToBeUpdated.FirstName : updatePlayerRequest.FirstName;
            playerToBeUpdated.LastName = string.IsNullOrWhiteSpace(updatePlayerRequest.LastName) ? playerToBeUpdated.LastName : updatePlayerRequest.LastName;
            playerToBeUpdated.Country = _countryHandler.IsCountryNameValid(updatePlayerRequest.Country) ? updatePlayerRequest.Country : playerToBeUpdated.Country;
            
            return playerToBeUpdated;
        }

        private async Task ValidatePlayerAsync(int playerId, int teamId)
        {
            var team = await _context.Teams.Include(t => t.Players).FirstAsync(t => t.Id == teamId);
            if (!team.Players.Any(p => p.Id == playerId))
            {
                throw new UnauthorizedPlayerActionException("The player does not exist or is not assigned to this team owner");
            }
        }
    }
}
