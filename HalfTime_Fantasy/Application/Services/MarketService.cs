using Application.Services.Interfaces;
using Contract.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class MarketService : IMarketService
    {
        private readonly IDbContext _context;

        public MarketService(IDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transfer>> GetTransferListAsync()
        {
            return await _context.Transfers.Include(t => t.Player).ToListAsync();
        }

        public async Task<Transfer> SellPlayerAsync(SellPlayerRequest sellPlayerRequest, int teamId)
        {
            if (await PlayerExistsAndPlaysForThisTeamAsync(sellPlayerRequest, teamId))
            {
                if (!await PlayerIsForSaleAsync(sellPlayerRequest.PlayerId))
                {
                    var transfer = new Transfer() { PlayerId = sellPlayerRequest.PlayerId, Value = sellPlayerRequest.Value };
                    await AddTransferToDatabaseAsync(transfer);
                    return transfer;
                }
                throw new UnauthorizedPlayerActionException("The player is already for sale");
            }
            else
            {
                throw new UnauthorizedPlayerActionException("The player is not assigned to this team owner");
            }

        }

        public async Task<Player> BuyPlayerAsync(BuyPlayerRequest buyPlayerRequest, int teamId)
        {
            var transfer = await GetTransferByPlayerIdAsync(buyPlayerRequest.PlayerId);
            if (transfer != null)
            {
                if (transfer.Player.TeamId != teamId)
                {
                    return await FinalizeSaleAsync(teamId, transfer);
                }
                else
                {
                    throw new UnauthorizedPlayerActionException("The player is already assigned to this team owner");
                }
            }
            else
            {
                throw new UnauthorizedPlayerActionException("The player is not for sale");
            }
        }

        private async Task<Player> FinalizeSaleAsync(int teamId, Transfer transfer)
        {
            var buyingTeam = await _context.Teams.FindAsync(teamId);
            var sellingTeam = transfer.Player.Team;
            var player = transfer.Player;

            if (BuyingTeamHasEnoughBudget(transfer, buyingTeam))
            {
                TransferPlayerBetweenTeams(transfer, buyingTeam, sellingTeam, player);
                await UpdateDatabaseAsync(transfer, buyingTeam, sellingTeam, player);
            }
            else
            {
                throw new UnauthorizedPlayerActionException("Not enough budget for this operation");
            }

            return player;
        }

        private static bool BuyingTeamHasEnoughBudget(Transfer transfer, Team buyingTeam)
        {
            return buyingTeam.Budget >= transfer.Value;
        }

        private async Task UpdateDatabaseAsync(Transfer transfer, Team buyingTeam, Team sellingTeam, Player player)
        {
            _context.Teams.Update(buyingTeam);
            _context.Teams.Update(sellingTeam);
            _context.Players.Update(player);
            _context.Transfers.Remove(transfer);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        private static void TransferPlayerBetweenTeams(Transfer transfer, Team buyingTeam, Team sellingTeam, Player player)
        {
            buyingTeam.Budget -= transfer.Value;
            sellingTeam.Budget += transfer.Value;
            sellingTeam.MarketValue -= player.MarketValue;

            var randomPercentage = new Random().Next(10, 100) / 100.00;
            player.MarketValue += player.MarketValue * Convert.ToDecimal(randomPercentage);
            buyingTeam.MarketValue += player.MarketValue;
            player.TeamId = buyingTeam.Id;
        }

        private async Task<Transfer> GetTransferByPlayerIdAsync(int playerId)
        {
            return await _context.Transfers.Include(t => t.Player).ThenInclude(p => p.Team).FirstAsync(t => t.PlayerId == playerId);
        }

        private async Task<bool> PlayerIsForSaleAsync(int playerId)
        {
            return await _context.Transfers.AnyAsync(t => t.PlayerId == playerId);
        }

        private async Task<bool> PlayerExistsAndPlaysForThisTeamAsync(SellPlayerRequest sellPlayerRequest, int teamId)
        {
            var team = await _context.Teams.Include(t => t.Players).FirstAsync(t => t.Id == teamId);
            if (!team.Players.Any(p => p.Id == sellPlayerRequest.PlayerId))
            {
                return false;
            }
            return true;
        }

        private async Task AddTransferToDatabaseAsync(Transfer transfer)
        {
            await _context.Transfers.AddAsync(transfer);
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
