using LiveOrderService.Application.Users;
using LiveOrderService.Common.Extensions;
using LiveOrderService.src.Application.Users;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGroup("/api", userRoute =>
{
    userRoute.MapGet("/", (GetAllUsersQueryHandler query) => query.Handle(new GetAllUsersQuery(), CancellationToken.None));
});

app.Run();