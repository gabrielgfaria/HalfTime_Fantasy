using Contract.Requests;
using Domain.Entities;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebUi.IntergationTests
{
    public class TeamControllerTest : TestBase
    {
        private const string GET_TEAM_ENDPOINT = "api/Team";
        private const string UPDATE_TEAM_ENDPOINT = "api/Team";

        [Fact]
        public async Task Get_WithoutAuthorizationToken_Returns401()
        {
            // Arrange

            // Act
            var result = await TestClient.GetAsync(GET_TEAM_ENDPOINT);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_WithAuthorizationToken_ReturnsTeam()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var responseData = JsonConvert.DeserializeObject<Team>(await response.Content.ReadAsStringAsync());

            // Assert
            responseData.ShouldNotBeNull();
        }

        [Fact]
        public async Task Patch_WithoutAuthorizationToken_Returns401()
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new UpdateTeamRequest() { Country = "", Name = "" }), Encoding.UTF8, "application/json");

            // Act
            var result = await TestClient.PatchAsync(UPDATE_TEAM_ENDPOINT, content);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Patch_WithAuthorizationToken_UpdatesTeam()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var updateRequest = new UpdateTeamRequest() { Country = "Brazil", Name = "Galo" };
            var content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_TEAM_ENDPOINT, content);
            getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            team = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());

            // Assert
            team.Name.ShouldBe(updateRequest.Name);
            team.Country.ShouldBe(updateRequest.Country);
        }

        [Fact]
        public async Task Patch_InvalidCountryName_DoesNotUpdateTeamCountry()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var teamBeforeUpdate = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var updateRequest = new UpdateTeamRequest() { Country = Guid.NewGuid().ToString(), Name = "Galo" };
            var content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_TEAM_ENDPOINT, content);
            getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var teamAfterUpdate = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());

            // Assert
            teamAfterUpdate.Country.ShouldBe(teamBeforeUpdate.Country);
            teamAfterUpdate.Name.ShouldBe(updateRequest.Name);
        }

        [Fact]
        public async Task Patch_EmptyName_DoesNotUpdateTeamName()
        {
            // Arrange
            await AuthenticateAsync();
            var getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var teamBeforeUpdate = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());
            var updateRequest = new UpdateTeamRequest() { Country = "Brazil", Name = "" };
            var content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

            // Act
            await TestClient.PatchAsync(UPDATE_TEAM_ENDPOINT, content);
            getTeamResponse = await TestClient.GetAsync(GET_TEAM_ENDPOINT);
            var teamAfterUpdate = JsonConvert.DeserializeObject<Team>(await getTeamResponse.Content.ReadAsStringAsync());

            // Assert
            teamAfterUpdate.Country.ShouldBe(updateRequest.Country);
            teamAfterUpdate.Name.ShouldBe(teamBeforeUpdate.Name);
        }
    }
}
