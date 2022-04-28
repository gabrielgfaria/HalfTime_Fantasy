using Contract.Requests;
using Domain.Entities;
using Domain.Enums;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebUi.IntergationTests
{
    public class MarketControllerTest : TestBase
    {
        private const string GET_MARKET_ENDPOINT = "api/Market";
        private const string SELL_PLAYER_ENDPOINT = "api/Market/Sell";
        private const string BUY_PLAYER_ENDPOINT = "api/Market/Buy";
        private const string GET_PLAYER_ENDPOINT = "api/Player/";
        private const string GET_TEAM_ENDPOINT = "api/Team";

        private const int TEAM_BUDGET = 5000000;
        private const int PLAYER_MARKET_VALUE = 1000000;
        private const int INITIAL_NUMBER_OF_PLAYERS = 20;
        private const int INITIAL_NUMBER_OF_GOALKEEPERS = 3;
        private const int CHEAP_PLAYER = 500000;
        private const int EXPENSIVE_PLAYER = 50000000;

        [Fact]
        public async Task Get_WithoutAuthorizationToken_Returns401()
        {
            // Arrange

            // Act
            var result = await TestClient.GetAsync(GET_MARKET_ENDPOINT);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_WithAuthorizationToken_Returns200()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var result = await TestClient.GetAsync(GET_MARKET_ENDPOINT);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Sell_WithoutAuthorizationToken_Returns401()
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new SellPlayerRequest() { PlayerId = 0, Value = 0 }), Encoding.UTF8, "application/json");

            // Act
            var result = await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, content);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Sell_WithAuthorizationToken_AddsPlayerToMarketList()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = CHEAP_PLAYER };
            var content = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, content);
            var getMarketResponse = await TestClient.GetAsync(GET_MARKET_ENDPOINT);
            var marketList = JsonConvert.DeserializeObject<List<Transfer>>(await getMarketResponse.Content.ReadAsStringAsync());

            // Assert
            getMarketResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            marketList.SingleOrDefault(p => p.PlayerId == sellRequest.PlayerId).ShouldNotBeNull();
            marketList.SingleOrDefault(p => p.Value == sellRequest.Value).ShouldNotBeNull();
        }

        [Fact]
        public async Task Sell_ZeroDolars_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = 0 };
            var content = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, content);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Sell_WithoutSpecifyingPlayer_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var sellRequest = new SellPlayerRequest() { Value = CHEAP_PLAYER };
            var content = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, content);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Sell_AnotherTeamsPlayer_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            await AuthenticateAsync();
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = CHEAP_PLAYER };
            var content = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, content);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Sell_PlayerAlreadyInMarketList_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = 500000 };
            var sellContent = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, sellContent);

            // Act
            var response = await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, sellContent);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Buy_WithoutAuthorizationToken_Returns401()
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new BuyPlayerRequest() { PlayerId = 0 }), Encoding.UTF8, "application/json");

            // Act
            var result = await TestClient.PostAsync(BUY_PLAYER_ENDPOINT, content);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Buy_Player_TransfersPlayerToTeam()
        {
            // Arrange
            var userAuthRequest = new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password123" };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);
            var loginResponse = await TestClient.PostAsync(LOGIN_USER_ENDPOINT, authContent);
            var tokenUser1 = await loginResponse.Content.ReadAsStringAsync();
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenUser1);

            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team1 = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team1.Players.First().Id, Value = 500000 };
            var sellContent = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, sellContent);

            await AuthenticateAsync();
            var buyContent = new StringContent(JsonConvert.SerializeObject(new BuyPlayerRequest() { PlayerId = team1.Players.First().Id }), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(BUY_PLAYER_ENDPOINT, buyContent);
            getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team2 = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var playerRequest = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + team1.Players.First().Id);
            var player = JsonConvert.DeserializeObject<Player>(await playerRequest.Content.ReadAsStringAsync());
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenUser1);
            response = await TestClient.PostAsync(BUY_PLAYER_ENDPOINT, buyContent);
            getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            team1 = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());

            // Assert
            team2.MarketValue.ShouldBe(team2.Players.Sum(p => p.MarketValue));
            team2.Budget.ShouldBe(TEAM_BUDGET - CHEAP_PLAYER);
            player.MarketValue.ShouldBeInRange(Convert.ToDecimal(PLAYER_MARKET_VALUE * 1.1), Convert.ToDecimal(PLAYER_MARKET_VALUE * 2));
            player.TeamId.ShouldBe(team2.Id);
            team2.Players.Count.ShouldBe(INITIAL_NUMBER_OF_PLAYERS + 1);
            team2.Players.Where(p => p.Position == PlayerPosition.Goalkeeper.ToString()).Count().ShouldBe(INITIAL_NUMBER_OF_GOALKEEPERS + 1);

            team1.MarketValue.ShouldBe(team1.Players.Sum(p => p.MarketValue));
            team1.Budget.ShouldBe(TEAM_BUDGET + CHEAP_PLAYER);
            team1.Players.Count.ShouldBe(INITIAL_NUMBER_OF_PLAYERS - 1);
            team1.Players.Where(p => p.Position == PlayerPosition.Goalkeeper.ToString()).Count().ShouldBe(INITIAL_NUMBER_OF_GOALKEEPERS - 1);
        }

        [Fact]
        public async Task Buy_TeamsOwnPlayer_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = CHEAP_PLAYER };
            var sellContent = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, sellContent);
            var buyContent = new StringContent(JsonConvert.SerializeObject(new BuyPlayerRequest() { PlayerId = team.Players.First().Id }), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(BUY_PLAYER_ENDPOINT, buyContent);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Buy_NotEnoughBudget_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = EXPENSIVE_PLAYER };
            var sellContent = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, sellContent);
            await AuthenticateAsync();
            var buyContent = new StringContent(JsonConvert.SerializeObject(new BuyPlayerRequest() { PlayerId = team.Players.First().Id }), Encoding.UTF8, "application/json");

            // Act
            var response = await TestClient.PostAsync(BUY_PLAYER_ENDPOINT, buyContent);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Buy_Player_RemovesPlayerFromTransferList()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var sellRequest = new SellPlayerRequest() { PlayerId = team.Players.First().Id, Value = 500000 };
            var sellContent = new StringContent(JsonConvert.SerializeObject(sellRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(SELL_PLAYER_ENDPOINT, sellContent);

            await AuthenticateAsync();
            var buyContent = new StringContent(JsonConvert.SerializeObject(new BuyPlayerRequest() { PlayerId = team.Players.First().Id }), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PostAsync(BUY_PLAYER_ENDPOINT, buyContent);
            var marketListRequest = await TestClient.GetAsync(GET_MARKET_ENDPOINT);
            var marketList = JsonConvert.DeserializeObject<List<Transfer>>(await marketListRequest.Content.ReadAsStringAsync());

            // Assert
            marketList.Where(m => m.PlayerId == team.Players.First().Id).Count().ShouldBe(0);
        }
    }
}
