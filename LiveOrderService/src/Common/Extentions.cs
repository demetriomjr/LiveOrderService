
namespace LiveOrderService.Common.Extensions
{
    public static class Extensions
    {
        public static IEndpointRouteBuilder MapGroup(
            this IEndpointRouteBuilder builder, 
            string path, 
            Action<IEndpointRouteBuilder> handle)
        {
            var route = builder.MapGroup(path);
            handle(route);
            return route;
        }
    } 
}