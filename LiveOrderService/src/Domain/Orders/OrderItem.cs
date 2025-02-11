
namespace Domain.Orders
{
    public class OrderItem : IBaseModel
    {
        public uint Id { get; set; }
        public IBaseModel.StatusOptions Status { get; set; } = IBaseModel.StatusOptions.ACTIVE;
        public required string Description { get; set; } 
        public decimal Value { get; set; } = 0;
        public decimal Amount { get; set; } = 0;
        public decimal TotalValue => Value * Amount;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
