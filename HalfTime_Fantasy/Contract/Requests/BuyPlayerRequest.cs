using System.ComponentModel.DataAnnotations;

namespace Contract.Requests
{
    public class BuyPlayerRequest
    {
        [Required(ErrorMessage = "It's necessary to specify a player")]
        public int PlayerId { get; set; }
    }
}
