using Application.Services;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests
{
    public class PlayerServiceTest
    {
        private IPlayerService _playerService;
        private readonly Mock<IDbContext> _dbContext = new Mock<IDbContext>();
        private readonly Mock<IPlayerNameHandler> _playerNameHandler = new Mock<IPlayerNameHandler>();
        private readonly Mock<ICountryNameHandler> _countryNameHandler = new Mock<ICountryNameHandler>();

        [Fact]
        public void CreatePlayers_ShouldCreate20Players()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.Count.ShouldBe(20);
            players.Where(p => p.Position == PlayerPosition.Attacker.ToString()).Count().ShouldBe(5);
        }

        [Fact]
        public void CreatePlayers_AllPlayersShouldHaveTheSpecifiedMarketValueOf1000000()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.All(p => p.MarketValue == 1000000).ShouldBe(true);
        }

        [Fact]
        public void CreatePlayers_ShouldCreate3Goalkeepers()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.Where(p => p.Position == PlayerPosition.Goalkeeper.ToString()).Count().ShouldBe(3);
        }

        [Fact]
        public void CreatePlayers_ShouldCreate6Defenders()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.Where(p => p.Position == PlayerPosition.Defender.ToString()).Count().ShouldBe(6);
        }

        [Fact]
        public void CreatePlayers_ShouldCreate6Midfielders()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.Where(p => p.Position == PlayerPosition.Midfielder.ToString()).Count().ShouldBe(6);
        }

        [Fact]
        public void CreatePlayers_ShouldCreate5Attackers()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.Where(p => p.Position == PlayerPosition.Attacker.ToString()).Count().ShouldBe(5);
        }

        [Fact]
        public void CreatePlayers_AllPlayersShouldBeBetween18and40YearsOld()
        {
            // Arrange
            _playerService = new PlayerService(_dbContext.Object, _playerNameHandler.Object, _countryNameHandler.Object);

            // Act
            var players = _playerService.CreatePlayers(new Team());

            // Assert
            players.Select(p => p.Age).All(a => a >= 18 && a <= 40).ShouldBe(true);
        }
    }
}
