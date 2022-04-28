using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.NameHandlers
{
    public class TeamNameHandler : ITeamNameHandler
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> _teamNames;

        public TeamNameHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _teamNames = File.ReadAllLines(_configuration.GetSection("TeamNamesFilePath").Value).ToList();
        }

        public string GenerateTeamName()
        {
            var randomIndex = new Random().Next(0, _teamNames.Count);
            return _teamNames[randomIndex];
        }
    }
}
