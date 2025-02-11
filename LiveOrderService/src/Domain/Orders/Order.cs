using Domain.Users;

namespace Domain.Orders
{
    public class Order : IBaseModel
    {
        public required uint Id { get; set; }
        public IBaseModel.StatusOptions Status { get; set; } = IBaseModel.StatusOptions.ACTIVE;
        public required IReadOnlyList<User> Users { get; init; }
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        
        private readonly List<OrderItem> _items = [];
        
        public decimal TotalItems => Items.Sum(x => x.Amount);
        public decimal TotalUniqueItems => Items.Count;
        public decimal TotalValue => Items.Sum(x => x.TotalValue);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Order(IEnumerable<User> editors)
        {
            Users = editors?.ToList() ?? throw new ArgumentNullException(nameof(editors));
            if (!Users.Any()) throw new ArgumentException("Editors cannot be empty", nameof(editors));
        }

        public void AddItem(OrderItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _items.Add(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            Status = IBaseModel.StatusOptions.DELETED;
            DeletedAt = DateTime.UtcNow;
        }

        public void ClearList()
        {
            _items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
