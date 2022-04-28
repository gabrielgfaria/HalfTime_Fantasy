using Contract.Requests;
using Domain.Entities;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebUi.IntergationTests
{
    public class PlayerControllerTest : TestBase
    {
        private const string UPDATE_PLAYER_ENDPOINT = "api/Player";
        private const string GET_PLAYER_ENDPOINT = "api/Player/";

        [Fact]
        public async Task Get_WithoutAuthorizationToken_Returns401()
        {
            // Arrange

            // Act
            var result = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + "1");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_WithAuthorizationToken_Returns200()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());

            // Act
            var result = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + team.Players.First().Id);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_AnotherTeamsPlayer_Returns400()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());
            await AuthenticateAsync();

            // Act
            var result = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + (team.Players.First().Id));

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Patch_WithoutAuthorizationToken_Returns401()
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = 0, Country = "", FirstName = "", LastName = "" }), Encoding.UTF8, "application/json");  ;

            // Act
            var result = await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Patch_WithAuthorizationToken_ChangesPlayerProperties()
        {
            // Arrange
            var updatedData = new Player() { Country = "Brazil", FirstName = "Gabriel", LastName = "Faria" };
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = team.Players.First().Id, Country = updatedData.Country, FirstName = updatedData.FirstName, LastName = updatedData.LastName }), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);
            var getPlayer = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + team.Players.First().Id);
            var result = JsonConvert.DeserializeObject<Player>(await getPlayer.Content.ReadAsStringAsync());

            // Assert
            result.FirstName.ShouldBe(updatedData.FirstName);
            result.LastName.ShouldBe(updatedData.LastName);
            result.Country.ShouldBe(updatedData.Country);
        }

        [Fact]
        public async Task Patch_WithInvalidCountry_DoesNotChangeCountry()
        {
            // Arrange
            var updatedData = new Player() { Country = Guid.NewGuid().ToString(), FirstName = "Gabriel", LastName = "Faria" };
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());
            var playerBeforeUpdate = team.Players.First();
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = playerBeforeUpdate.Id, Country = updatedData.Country, FirstName = updatedData.FirstName, LastName = updatedData.LastName }), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);
            var getPlayer = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + team.Players.First().Id);
            var result = JsonConvert.DeserializeObject<Player>(await getPlayer.Content.ReadAsStringAsync());

            // Assert
            result.FirstName.ShouldBe(updatedData.FirstName);
            result.LastName.ShouldBe(updatedData.LastName);
            result.Country.ShouldBe(playerBeforeUpdate.Country);
        }

        [Fact]
        public async Task Patch_WithEmptyFirstName_DoesNotChangeFirstName()
        {
            // Arrange
            var updatedData = new Player() { Country = "Brazil", FirstName = "", LastName = "Faria" };
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());
            var playerBeforeUpdate = team.Players.First();
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = playerBeforeUpdate.Id, Country = updatedData.Country, FirstName = updatedData.FirstName, LastName = updatedData.LastName }), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);
            var getPlayer = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + team.Players.First().Id);
            var result = JsonConvert.DeserializeObject<Player>(await getPlayer.Content.ReadAsStringAsync());

            // Assert
            result.FirstName.ShouldBe(playerBeforeUpdate.FirstName);
            result.LastName.ShouldBe(updatedData.LastName);
            result.Country.ShouldBe(updatedData.Country);
        }

        [Fact]
        public async Task Patch_WithEmptyLastName_DoesNotChangeLastName()
        {
            // Arrange
            var updatedData = new Player() { Country = "Brazil", FirstName = "Gabriel", LastName = "" };
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());
            var playerBeforeUpdate = team.Players.First();
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = playerBeforeUpdate.Id, Country = updatedData.Country, FirstName = updatedData.FirstName, LastName = updatedData.LastName }), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);
            var getPlayer = await TestClient.GetAsync(GET_PLAYER_ENDPOINT + team.Players.First().Id);
            var result = JsonConvert.DeserializeObject<Player>(await getPlayer.Content.ReadAsStringAsync());

            // Assert
            result.FirstName.ShouldBe(updatedData.FirstName);
            result.LastName.ShouldBe(playerBeforeUpdate.LastName);
            result.Country.ShouldBe(updatedData.Country);
        }

        [Fact]
        public async Task Patch_AnotherTeamsPlayer_Returns400()
        {
            // Arrange
            var updatedData = new Player() { Country = "Brazil", FirstName = "Gabriel", LastName = "Faria" };
            await AuthenticateAsync();
            var getTeam = await TestClient.GetAsync("api/Team");
            var team = JsonConvert.DeserializeObject<Team>(await getTeam.Content.ReadAsStringAsync());
            await AuthenticateAsync();
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = team.Players.First().Id, Country = updatedData.Country, FirstName = updatedData.FirstName, LastName = updatedData.LastName }), Encoding.UTF8, "application/json");

            // Act
            var result = await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Patch_UnexistingPlayer_Returns400()
        {
            // Arrange
            var updatedData = new Player() { Country = "Brazil", FirstName = "Gabriel", LastName = "Faria" };
            await AuthenticateAsync();
            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePlayerRequest() { PlayerId = int.MaxValue, Country = updatedData.Country, FirstName = updatedData.FirstName, LastName = updatedData.LastName }), Encoding.UTF8, "application/json");

            // Act
            var result = await TestClient.PatchAsync(UPDATE_PLAYER_ENDPOINT, content);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
