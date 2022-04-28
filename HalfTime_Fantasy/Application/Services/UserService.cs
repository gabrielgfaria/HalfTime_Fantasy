using Application.Services.Interfaces;
using Contract.Responses;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContext _context;

        public UserService(IDbContext context)
        {
            _context = context;
        }

        public UserResponse Get(int userId)
        {
            var user = _context.Users.Include(u => u.Team).ThenInclude(t => t.Players).ThenInclude(p => p.Transfer).SingleOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var userResponse = new UserResponse()
                {
                    Id = user.Id,
                    TeamId = user.TeamId,
                    Team = user.Team,
                    Email = user.Email,
                    UserRole = user.UserRole
                };
                return userResponse;
            }
            else
            {
                throw new UserNotFoundException("The specified user does not exist");
            }
        }

        public async Task DeleteAsync(int userId)
        {
            var user = _context.Users.Include(u => u.Team).ThenInclude(t => t.Players).ThenInclude(p => p.Transfer).SingleOrDefault(x => x.Id == userId);
            if (user != null && user.UserRole != UserRole.Admin.ToString())
            {
                var transfers = new List<Transfer>();
                foreach (var player in user.Team.Players)
                {
                    if (player.Transfer != null)
                    {
                        transfers.Add(player.Transfer);
                    }
                }
                if (transfers.Any())
                {
                    _context.Transfers.RemoveRange(transfers);
                }
                _context.Players.RemoveRange(user.Team.Players);
                _context.Teams.Remove(user.Team);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(CancellationToken.None);
            }
            else
            {
                throw new UserNotFoundException("The specified user does not exist or there's not enough permission to delete it");
            }
        }
    }
}
