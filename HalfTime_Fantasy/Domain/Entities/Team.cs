using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public partial class Team
    {
        public Team()
        {
            Players = new HashSet<Player>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public decimal Budget { get; set; }
        public decimal MarketValue { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
        public virtual ICollection<Player> Players { get; set; }
    }
}
