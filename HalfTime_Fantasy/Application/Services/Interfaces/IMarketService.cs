using Contract.Requests;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IMarketService
    {
        Task<Transfer> SellPlayerAsync(SellPlayerRequest sellPlayerRequest, int teamId);
        Task<Player> BuyPlayerAsync(BuyPlayerRequest buyPlayerRequest, int teamId);
        Task<List<Transfer>> GetTransferListAsync();
    }
}