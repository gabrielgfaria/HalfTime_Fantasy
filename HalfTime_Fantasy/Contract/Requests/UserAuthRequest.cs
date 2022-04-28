using System.ComponentModel.DataAnnotations;

namespace Contract.Requests
{
    public class UserAuthRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "An email address is required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; }
    }
}
