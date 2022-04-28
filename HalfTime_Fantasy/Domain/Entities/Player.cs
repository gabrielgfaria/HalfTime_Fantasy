using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public partial class Player
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public decimal MarketValue { get; set; }

        [JsonIgnore]
        public virtual Team Team { get; set; }
        [JsonIgnore]
        public virtual Transfer Transfer { get; set; }
    }
}
