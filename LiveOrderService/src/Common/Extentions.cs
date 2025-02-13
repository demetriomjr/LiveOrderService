using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Common.Extensions
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