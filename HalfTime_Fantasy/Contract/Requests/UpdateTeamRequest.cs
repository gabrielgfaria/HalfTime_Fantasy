using System.ComponentModel.DataAnnotations;

namespace Contract.Requests
{
    public class UpdateTeamRequest
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
