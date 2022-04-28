using Application.Services;
using Application.Services.Interfaces;
using Infrastructure.NameHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HalfTime_FantasyContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(HalfTime_FantasyContext).Assembly.FullName)));

            // DbContext
            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<HalfTime_FantasyContext>());

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IMarketService, MarketService>();
            services.AddScoped<IUserService, UserService>();

            // Common
            services.AddSingleton<IPlayerNameHandler, PlayerNameHandler>();
            services.AddSingleton<ICountryNameHandler, CountryNameHandler>();
            services.AddSingleton<ITeamNameHandler, TeamNameHandler>();
            
            return services;
        }
    }
}
