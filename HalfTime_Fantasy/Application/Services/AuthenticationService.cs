using Application.Common;
using Application.Services.Interfaces;
using Contract.Requests;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDbContext _context;
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IDbContext context,
            ITeamService teamService,
            IPlayerService playerService,
            IConfiguration configuration)
        {
            _context = context;
            _teamService = teamService;
            _playerService = playerService;
            _configuration = configuration;
        }

        public async Task RegisterAsync(UserAuthRequest userRequest)
        {
            ValidateUser(userRequest);
            CreatePasswordHash(userRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
            
            var team = await CreateTeamAndPlayersAsync();
            await CreateNewUserAsync(userRequest, team, passwordHash, passwordSalt);
        }

        public string Login(UserAuthRequest userRequest)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userRequest.Email);
            if (user == null || !GetHash(userRequest.Password, user.PasswordSalt).SequenceEqual(user.PasswordHash))
            {
                throw new UserNotFoundException("Invalid email or password");
            }
            return GenerateToken(user);
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Secret").Value.ToString());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(CustomClaimTypes.TeamId, user.TeamId.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<Team> CreateTeamAndPlayersAsync()
        {
            var team = _teamService.CreateTeam();
            _context.Teams.Add(team);
            await _context.SaveChangesAsync(CancellationToken.None);
            var players = _playerService.CreatePlayers(team);
            _context.Players.AddRange(players);
            return team;
        }

        private byte[] GetHash(string password, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private async Task CreateNewUserAsync(UserAuthRequest userRequest, Team team, byte[] passwordHash, byte[] passwordSalt)
        {
            var user = new User();
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.Email = userRequest.Email;
            user.TeamId = team.Id;
            user.UserRole = UserRole.TeamOwner.ToString();
            _context.Users.Add(user);
            
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        private void ValidateUser(UserAuthRequest userRequest)
        {
            if (_context.Users.FirstOrDefault(e => e.Email == userRequest.Email) != null)
            {
                throw new ExistingUserException("There already is an user registered with this email");
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
