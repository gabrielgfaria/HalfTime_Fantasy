using Contract.Requests;
using Domain.Entities;
using Domain.Enums;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebUi.IntergationTests
{
    public class AuthenticationControllerTest : TestBase
    {
        private const int TEAM_BUDGET = 5000000;
        private const int PLAYER_MARKET_VALUE = 1000000;
        private const int PLAYER_MINIMAL_AGE = 18;
        private const int PLAYER_MAX_AGE = 40;
        private const int INITIAL_NUMBER_OF_PLAYERS = 20;
        private const int INITIAL_NUMBER_OF_GOALKEEPERS = 3;
        private const int INITIAL_NUMBER_OF_DEFENDERS = 6;
        private const int INITIAL_NUMBER_OF_MIDFIELDERS = 6;
        private const int INITIAL_NUMBER_OF_ATTACKERS = 5;

        [Fact]
        public async Task Register_ExistingUser_Returns400()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Act
            var response = await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_InvalidEmail_Returns400()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Act
            var response = await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_EmptyPassword_Returns400()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Act
            var response = await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_NonExistingUser_Returns200()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_NonExistingUser_ShouldCreateAValidTeam()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);
            var loginResponse = await TestClient.PostAsync(LOGIN_USER_ENDPOINT, authContent);
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await loginResponse.Content.ReadAsStringAsync());

            // Act
            var response = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await response.Content.ReadAsStringAsync());

            // Assert
            team.ShouldNotBeNull();
            team.MarketValue.ShouldBe(team.Players.Sum(p => p.MarketValue));
            team.Budget.ShouldBe(TEAM_BUDGET);
            team.Players.All(p => p.MarketValue == PLAYER_MARKET_VALUE).ShouldBe(true);
            team.Players.Count.ShouldBe(INITIAL_NUMBER_OF_PLAYERS);
            team.Players.All(p => p.Age >= PLAYER_MINIMAL_AGE && p.Age <= PLAYER_MAX_AGE).ShouldBe(true);
            team.Players.All(p => p.TeamId == team.Id).ShouldBe(true);
            team.Players.Where(p => p.Position == PlayerPosition.Goalkeeper.ToString()).Count().ShouldBe(INITIAL_NUMBER_OF_GOALKEEPERS);
            team.Players.Where(p => p.Position == PlayerPosition.Defender.ToString()).Count().ShouldBe(INITIAL_NUMBER_OF_DEFENDERS);
            team.Players.Where(p => p.Position == PlayerPosition.Midfielder.ToString()).Count().ShouldBe(INITIAL_NUMBER_OF_MIDFIELDERS);
            team.Players.Where(p => p.Position == PlayerPosition.Attacker.ToString()).Count().ShouldBe(INITIAL_NUMBER_OF_ATTACKERS);
        }

        [Fact]
        public async Task Login_NonExistingUser_Returns404()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(LOGIN_USER_ENDPOINT, authContent);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Login_ExistingUser_Returns200()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            // Act
            var response = await TestClient.PostAsync(LOGIN_USER_ENDPOINT, authContent);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }
    }
}
