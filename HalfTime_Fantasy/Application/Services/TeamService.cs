using Application.Common;
using Application.Services.Interfaces;
using Contract.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TeamService : ITeamService
    {
        private const decimal INITIAL_TEAM_BUDGET = 5000000;
        private const decimal INITIAL_TEAM_VALUE = 20000000;

        private readonly IDbContext _context;
        private readonly ICountryNameHandler _countryNameGenerator;
        private readonly ITeamNameHandler _teamNameGenerator;

        public TeamService(IDbContext context,
            ICountryNameHandler countryGenerator,
            ITeamNameHandler teamNameGenerator)
        {
            _context = context;
            _countryNameGenerator = countryGenerator;
            _teamNameGenerator = teamNameGenerator;
        }

        public async Task<Team> GetTeamAsync(int teamId)
        {
            return await _context.Teams.Include(t => t.Players).FirstAsync(t => t.Id == teamId);
        }

        public async Task<Team> UpdateAsync(UpdateTeamRequest updateTeamRequest, int teamId)
        {
            var teamToUpdate = ConstructTeamToUpdate(updateTeamRequest, teamId);
            await UpdateDatabaseAsync(teamToUpdate);

            return teamToUpdate;
        }

        public Team CreateTeam()
        {
            var team = new Team();
            team.Name = _teamNameGenerator.GenerateTeamName();
            team.Country = _countryNameGenerator.GenerateCountryName();
            team.Budget = INITIAL_TEAM_BUDGET;
            team.MarketValue = INITIAL_TEAM_VALUE;

            return team;
        }

        private async Task UpdateDatabaseAsync(Team teamToUpdate)
        {
            _context.Teams.Update(teamToUpdate);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        private Team ConstructTeamToUpdate(UpdateTeamRequest updateTeamRequest, int teamId)
        {
            var existingTeam = _context.Teams.FirstOrDefault(x => x.Id == teamId);
            existingTeam.Name = string.IsNullOrWhiteSpace(updateTeamRequest.Name) ? existingTeam.Name : updateTeamRequest.Name;
            existingTeam.Country = _countryNameGenerator.IsCountryNameValid(updateTeamRequest.Country) ? updateTeamRequest.Country : existingTeam.Country;

            return existingTeam;
        }
    }
}
