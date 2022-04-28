using Application.Services;
using Application.Services.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace Application.UnitTests
{
    public class TeamServiceUnitTest
    {
        private ITeamService _teamService;
        private readonly Mock<IDbContext> _dbContext = new Mock<IDbContext>();
        private readonly Mock<ITeamNameHandler> _teamNameHandler = new Mock<ITeamNameHandler>();
        private readonly Mock<ICountryNameHandler> _countryNameHandler = new Mock<ICountryNameHandler>();

        [Fact]
        public void CreateTeam_ShouldCreateTeamWithSpecifiedBudgetOf5000000()
        {
            // Arrange
            _teamService = new TeamService(_dbContext.Object, _countryNameHandler.Object, _teamNameHandler.Object);

            // Act
            var team = _teamService.CreateTeam();

            // Assert
            team.Budget.ShouldBe(5000000);
        }

        [Fact]
        public void CreateTeam_ShouldCreateTeamWithSpecifiedMarketValyeOf20000000()
        {
            // Arrange
            _teamService = new TeamService(_dbContext.Object, _countryNameHandler.Object, _teamNameHandler.Object);

            // Act
            var team = _teamService.CreateTeam();

            // Assert
            team.MarketValue.ShouldBe(20000000);
        }
    }
}
