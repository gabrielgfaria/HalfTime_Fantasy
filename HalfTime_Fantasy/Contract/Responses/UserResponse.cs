using Domain.Entities;

namespace Contract.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int TeamId { get; set; }
        public string UserRole { get; set; }

        public virtual Team Team { get; set; }
    }
}
