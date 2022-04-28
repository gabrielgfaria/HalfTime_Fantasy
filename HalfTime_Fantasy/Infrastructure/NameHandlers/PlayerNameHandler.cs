using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.NameHandlers
{
    public class PlayerNameHandler : IPlayerNameHandler
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> _playerFirstNames;
        private readonly List<string> _playerLastNames;

        public PlayerNameHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _playerFirstNames = File.ReadAllLines(_configuration.GetSection("PlayerFirstNamesFilePath").Value).ToList();
            _playerLastNames = File.ReadAllLines(_configuration.GetSection("PlayerLastNamesFilePath").Value).ToList();
        }

        public string GenerateFirstName()
        {
            var randomIndex = new Random().Next(0, _playerFirstNames.Count);
            return _playerFirstNames[randomIndex];
        }

        public string GenerateLastName()
        {
            var randomIndex = new Random().Next(0, _playerLastNames.Count);
            return _playerLastNames[randomIndex];
        }
    }
}
