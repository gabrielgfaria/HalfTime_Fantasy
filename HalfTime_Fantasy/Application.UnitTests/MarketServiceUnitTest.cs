using Application.Services;
using Application.Services.Interfaces;
using Domain.Entities;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Application.UnitTests
{
    public class MarketServiceUnitTest
    {
        private IMarketService _marketService;
        private readonly Mock<IDbContext> _dbContext = new Mock<IDbContext>();

        public MarketServiceUnitTest()
        {
            _marketService = new MarketService(_dbContext.Object);
        }

        [Fact]
        public void TransferPlayerBetweenTeams_ShouldChangeThePlayersTeamId()
        {
            // Arrange
            var player = new Player() { Id = 1, MarketValue = 1000000, TeamId = 1 };
            var sellingTeam = new Team() { Id = 1, MarketValue = 1, Players = new List<Player>() { player }, Budget = 100 };
            var buyingTeam = new Team() { Id = 2, MarketValue = 1, Players = new List<Player>(), Budget = 100 };
            var transfer = new Transfer() { Player = player, PlayerId = 1, Value = 100 };

            var methodInfo = _marketService.GetType().GetMethod("TransferPlayerBetweenTeams", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam, sellingTeam, player };

            // Act
            methodInfo.Invoke(_marketService, parameters);

            // Assert
            player.TeamId.ShouldBe(2);
        }

        [Fact]
        public void TransferPlayerBetweenTeams_ShouldChangeThePlayersMarketValue()
        {
            // Arrange
            var initialPlayerMarketValue = 1000000;
            var player = new Player() { Id = 1, MarketValue = initialPlayerMarketValue, TeamId = 1 };
            var sellingTeam = new Team() { Id = 1, MarketValue = 1, Players = new List<Player>() { player }, Budget = 100 };
            var buyingTeam = new Team() { Id = 2, MarketValue = 1, Players = new List<Player>(), Budget = 100 };
            var transfer = new Transfer() { Player = player, PlayerId = 1, Value = 100 };

            var methodInfo = _marketService.GetType().GetMethod("TransferPlayerBetweenTeams", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam, sellingTeam, player };

            // Act
            methodInfo.Invoke(_marketService, parameters);

            // Assert
            player.MarketValue.ShouldBeInRange(Convert.ToDecimal(initialPlayerMarketValue * 1.1), Convert.ToDecimal(2 * initialPlayerMarketValue));
        }

        [Fact]
        public void TransferPlayerBetweenTeams_ShouldChangeBuyingTeamsBudget()
        {
            // Arrange
            var initialBudget = 1000000;
            var playerPrice = 500000;
            var player = new Player() { Id = 1, MarketValue = 1000000, TeamId = 1 };
            var sellingTeam = new Team() { Id = 1, MarketValue = 1, Players = new List<Player>() { player }, Budget = 100 };
            var buyingTeam = new Team() { Id = 2, MarketValue = 1, Players = new List<Player>(), Budget = initialBudget };
            var transfer = new Transfer() { Player = player, PlayerId = 1, Value = playerPrice };

            var methodInfo = _marketService.GetType().GetMethod("TransferPlayerBetweenTeams", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam, sellingTeam, player };

            // Act
            methodInfo.Invoke(_marketService, parameters);

            // Assert
            buyingTeam.Budget.ShouldBe(initialBudget-playerPrice);
        }

        [Fact]
        public void TransferPlayerBetweenTeams_ShouldChangeSellingTeamsBudget()
        {
            // Arrange
            var initialBudget = 1000000;
            var playerPrice = 500000;
            var player = new Player() { Id = 1, MarketValue = 1000000, TeamId = 1 };
            var sellingTeam = new Team() { Id = 1, MarketValue = 1, Players = new List<Player>() { player }, Budget = initialBudget };
            var buyingTeam = new Team() { Id = 2, MarketValue = 1, Players = new List<Player>(), Budget = initialBudget };
            var transfer = new Transfer() { Player = player, PlayerId = 1, Value = playerPrice };

            var methodInfo = _marketService.GetType().GetMethod("TransferPlayerBetweenTeams", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam, sellingTeam, player };

            // Act
            methodInfo.Invoke(_marketService, parameters);

            // Assert
            sellingTeam.Budget.ShouldBe(initialBudget + playerPrice);
        }

        [Fact]
        public void TransferPlayerBetweenTeams_ShouldChangeSellingTeamsMarketValue()
        {
            // Arrange
            var initialMarketValue = 1000000;
            var playerMarketValue = 500000;
            var player = new Player() { Id = 1, MarketValue = playerMarketValue, TeamId = 1 };
            var sellingTeam = new Team() { Id = 1, MarketValue = initialMarketValue, Players = new List<Player>() { player }, Budget = 1000000 };
            var buyingTeam = new Team() { Id = 2, MarketValue = 1, Players = new List<Player>(), Budget = 1000000 };
            var transfer = new Transfer() { Player = player, PlayerId = 1, Value = 1000000 };

            var methodInfo = _marketService.GetType().GetMethod("TransferPlayerBetweenTeams", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam, sellingTeam, player };

            // Act
            methodInfo.Invoke(_marketService, parameters);

            // Assert
            sellingTeam.MarketValue.ShouldBe(initialMarketValue - playerMarketValue);
        }

        [Fact]
        public void TransferPlayerBetweenTeams_ShouldChangeBuyingTeamsMarketValue()
        {
            // Arrange
            var initialMarketValue = 1000000;
            var player = new Player() { Id = 1, MarketValue = 5000000, TeamId = 1 };
            var sellingTeam = new Team() { Id = 1, MarketValue = initialMarketValue, Players = new List<Player>() { player }, Budget = 1000000 };
            var buyingTeam = new Team() { Id = 2, MarketValue = initialMarketValue, Players = new List<Player>(), Budget = 1000000 };
            var transfer = new Transfer() { Player = player, PlayerId = 1, Value = 1000000 };

            var methodInfo = _marketService.GetType().GetMethod("TransferPlayerBetweenTeams", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam, sellingTeam, player };

            // Act
            methodInfo.Invoke(_marketService, parameters);

            // Assert
            buyingTeam.MarketValue.ShouldBe(initialMarketValue + player.MarketValue);
        }

        [Fact]
        public void BuyingTeamHasEnoughBudget_ShouldReturnFalseWhenBuyingTeamDoesNotHaveEnoughCredit()
        {
            // Arrange
            var transfer = new Transfer() { Value = 5000000 };
            var buyingTeam = new Team() { Budget = 100000 };

            var methodInfo = _marketService.GetType().GetMethod("BuyingTeamHasEnoughBudget", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam };

            // Act
            var result = methodInfo.Invoke(_marketService, parameters);

            // Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void BuyingTeamHasEnoughBudget_ShouldReturnTrueWhenBuyingTeamHasEnoughCredit()
        {
            // Arrange
            var transfer = new Transfer() { Value = 100000 };
            var buyingTeam = new Team() { Budget = 5000000 };

            var methodInfo = _marketService.GetType().GetMethod("BuyingTeamHasEnoughBudget", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { transfer, buyingTeam };

            // Act
            var result = methodInfo.Invoke(_marketService, parameters);

            // Assert
            result.ShouldBe(true);
        }
    }
}
