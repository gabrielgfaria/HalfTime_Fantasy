using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.NameHandlers
{
    public class CountryNameHandler : ICountryNameHandler
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> _countryNames;

        public CountryNameHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _countryNames = File.ReadAllLines(_configuration.GetSection("CountryNamesFilePath").Value).ToList();
        }

        public string GenerateCountryName()
        {
            var randomIndex = new Random().Next(0, _countryNames.Count);
            return _countryNames[randomIndex];
        }

        public string GetCountryName(string country)
        {
            return _countryNames.Single(countryName => countryName == country);
        }

        public bool IsCountryNameValid(string countryName)
        {
            return _countryNames.Any(existingCountry => existingCountry.Equals(countryName.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
