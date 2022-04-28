
using Contract.Responses;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        UserResponse Get(int userId);
        Task DeleteAsync(int userId);
    }
}