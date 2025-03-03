namespace LiveOrderService.Domain.Tokens
{
    public record Token(string TokenKey, DateTimeOffset Expiration);
}