using LiveOrderService.Application.Users;
using LiveOrderService.Common.Extensions;
using LiveOrderService.src.Application.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGroup("/user", userRoute =>
{
    userRoute.MapGet("/", async (IMediator mediator, CancellationToken ct) => 
    {
        var query = new GetAllUsersQuery();
        return await mediator.Send(query, ct);
    });

    userRoute.MapGet("/{id:uint}", async (IMediator mediator, uint id, CancellationToken ct) => 
    {
        var query = new GetUserByIdQuery(id);
        return await mediator.Send(query, ct);
    });

    userRoute.MapGet("/{username:string}", async (IMediator mediator, string username, CancellationToken ct) => 
    {
        var query = new GetUserByUsernameQuery(username);
        return await mediator.Send(query, ct);
    });

    userRoute.MapPost("/", async (IMediator mediator, CancellationToken ct, [FromBody]CreateUserCommand command) => 
    {
        var result = await mediator.Send(command, ct);

        if(result is null)
            return Results.BadRequest("error while creating new user");

        return Results.Created($"/users/{result.Id}", result);
    });

    userRoute.MapPut("/", async (IMediator mediator, CancellationToken ct, [FromBody]UpdateUserCommand command) => 
    {
        var result = await mediator.Send(command, ct);
        return Results.Ok($"Return status code of {result}");
    });

    userRoute.MapPut("/{id:uint}", async (IMediator mediator, CancellationToken ct, uint id, [FromBody]UpdateUserCommand command) => 
    {
        if(!command.Id.Equals(id))
            return Results.BadRequest("Conflict of Id values");

        var result = await mediator.Send(command, ct);
        return Results.Ok($"Return status code of {result}");
    });

    userRoute.MapDelete("/{id:uint}", async (IMediator mediator, uint id, CancellationToken ct) => 
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, ct);
        return Results.Ok($"Return status code of {result}");
    });

});

app.Run();