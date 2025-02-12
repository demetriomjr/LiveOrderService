var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var users = app.MapGroup("/users");
users.MapGet("/", () => 
{
    return userService.GetUsers();
});

users.MapGet("/{id:uint}", (int id) => 
{
    return userService.GetUser(id);
});

users.MapGet("/{username:string}", (string username) => 
{
    return userService.GetUser(id);
});

users.MapPost("/", (User user) => 
{
    return userService.AddUser(user);
});

users.MapPut("/{id}", (int id, User user) => 
{
    return userService.UpdateUser(id, user);
});

users.MapDelete("/{id}", (int id) => 
{
    return userService.DeleteUser(id);
});

app.Run();