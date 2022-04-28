using Contract.Requests;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebUI;

namespace WebUi.IntergationTests
{
    public class TestBase
    {
        protected const string REGISTER_USER_ENDPOINT = "api/Authentication/register";
        protected const string LOGIN_USER_ENDPOINT = "api/Authentication/login";
        protected readonly HttpClient TestClient;
        
        public TestBase()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(DbContextOptions<HalfTime_FantasyContext>));

                        services.Remove(descriptor);

                        services.AddDbContext<HalfTime_FantasyContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        });
                    });
                });

            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            var teste = await GetJwtAsync(new UserAuthRequest() { Email = $"{Guid.NewGuid()}@email.com", Password = "password" });
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", teste);
        }

        private async Task<string> GetJwtAsync(UserAuthRequest user)
        {
            var userAuthRequest = new UserAuthRequest() { Email = user.Email, Password = user.Password };
            var authContent = new StringContent(JsonConvert.SerializeObject(userAuthRequest), Encoding.UTF8, "application/json");
            await TestClient.PostAsync(REGISTER_USER_ENDPOINT, authContent);

            var response = await TestClient.PostAsync(LOGIN_USER_ENDPOINT, authContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
