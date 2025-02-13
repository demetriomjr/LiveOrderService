namespace LiveOrderService.Domain.Interfaces
{
    public interface IBaseModel
{
    public enum StatusOptions
    {
        DELETED = -1,
        INACTIVE = 0,
        ACTIVE = 1,
    }

    public uint Id {get; set;}
    public StatusOptions Status {get; set;}
    public DateTime CreatedAt {get; init;}
    public DateTime? UpdatedAt {get; set;}
    public DateTime? DeletedAt {get; set;}
}
}