using Contract.Requests;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface ITeamService
    {
        Task<Team> GetTeamAsync(int teamId);
        Team CreateTeam();
        Task<Team> UpdateAsync(UpdateTeamRequest updateTeamRequest, int teamId);
    }
}