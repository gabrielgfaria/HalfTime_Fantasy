namespace Domain.Entities
{
    public partial class Transfer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public decimal Value { get; set; }

        public virtual Player Player { get; set; }
    }
}
