using System.ComponentModel.DataAnnotations;

namespace Contract.Requests
{
    public class UpdatePlayerRequest
    {
        [Required(ErrorMessage = "It's necessary to specify the player")]
        public int PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
    }
}
