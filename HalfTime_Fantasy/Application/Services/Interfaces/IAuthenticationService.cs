using Contract.Requests;

namespace Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task RegisterAsync(UserAuthRequest user);
        string Login(UserAuthRequest user);
    }
}