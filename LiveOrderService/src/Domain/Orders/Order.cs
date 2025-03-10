using LiveOrderService.Domain.Interfaces;
using LiveOrderService.Domain.Users;
using LiveOrderService.Utils;

namespace LiveOrderService.Domain.Orders
{
    public class Order(IEnumerable<User> users) : IBaseModel
    {
        public required uint Id { get; set; } = IdGenerator.GenerateId();
        public IBaseModel.StatusOptions Status { get; set; } = IBaseModel.StatusOptions.ACTIVE;
        public List<User> Users { get; private set; } = users.ToList();
        public List<OrderItem> Items {get; private set;} = [];   
        public decimal TotalItems => Items.Sum(x => x.Amount);
        public decimal TotalUniqueItems => Items.Count;
        public decimal TotalValue => Items.Sum(x => x.TotalValue);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void AddItem(OrderItem item)
        {
            if(Items.FirstOrDefault(x => x.Id == item.Id) is OrderItem existingItem)
            {
                existingItem.Amount += item.Amount;
                UpdatedAt = DateTime.UtcNow;
                return;
            }
            Items.Add(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            Status = IBaseModel.StatusOptions.DELETED;
            DeletedAt = DateTime.UtcNow;
        }

        public void ClearList()
        {
            Items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddUser(User user)
        {
            if (Users.Any(x => x.Id == user.Id))
                return;

            Users.Add(user);
            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveUser(User user)
        {
            if (!Users.Any(x => x.Id == user.Id))
                return;

            Users.Remove(user);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
