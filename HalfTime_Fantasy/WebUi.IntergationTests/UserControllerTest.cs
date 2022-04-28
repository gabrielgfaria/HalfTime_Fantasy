using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebUi.IntergationTests
{
    public class UserControllerTest : TestBase
    {
        private const string GET_USER_ENDPOINT = "api/User/";
        private const string DELETE_USER_ENDPOINT = "api/User/";

        [Fact]
        public async Task Get_WithoutAuthorization_Returns401()
        {
            // Arrange

            // Act
            var result = await TestClient.GetAsync(GET_USER_ENDPOINT + "1");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_WithTeamOwnerAuthorization_Returns403()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var result = await TestClient.GetAsync(GET_USER_ENDPOINT + "1");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_WithoutAuthorization_Returns401()
        {
            // Arrange

            // Act
            var result = await TestClient.DeleteAsync(DELETE_USER_ENDPOINT + "1");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_WithTeamOwnerAuthorization_Returns403()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var result = await TestClient.DeleteAsync(DELETE_USER_ENDPOINT + "1");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }
    }
}
