using System.ComponentModel.DataAnnotations;

namespace Contract.Requests
{
    public class SellPlayerRequest
    {
        [Required(ErrorMessage = "It's necessary to specify a player")]
        public int PlayerId { get; set; }

        [Required(ErrorMessage = "It's necessary to specify the selling value for the player")]
        [Range(1, int.MaxValue, ErrorMessage = "The price tag for a player must be greater than 0")]
        public decimal Value { get; set; }
    }
}
