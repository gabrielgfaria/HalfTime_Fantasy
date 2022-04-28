namespace Domain.Entities
{
    public partial class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int TeamId { get; set; }
        public string UserRole { get; set; }

        public virtual Team Team { get; set; }
    }
}
