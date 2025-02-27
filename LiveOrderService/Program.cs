using LiveOrderService.Application.DTOs;
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
        var result = await mediator.Send(query, ct);

        if(result.IsFailure)
            return Results.BadRequest(result.Error);
        
        if(result.Value is IEnumerable<UserResponseDto> users)
            return Results.Ok(users);

        return Results.InternalServerError();
    });

    userRoute.MapGet("/{id:uint}", async (IMediator mediator, uint id, CancellationToken ct) => 
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, ct);

        if(result.IsFailure)
            return Results.BadRequest(result.Error);    
        
        if(result.Value is UserResponseDto user)
            return Results.Ok(user);

        return Results.InternalServerError();
    });

    userRoute.MapGet("/{username:string}", async (IMediator mediator, string username, CancellationToken ct) => 
    {
        var query = new GetUserByUsernameQuery(username);
        var result = await mediator.Send(query, ct);

        if(result.IsFailure)
            return Results.BadRequest(result.Error);    
        
        if(result.Value is UserResponseDto user)
            return Results.Ok(user);

        return Results.InternalServerError();
    });

    userRoute.MapPost("/", async (IMediator mediator, CancellationToken ct, [FromBody]CreateUserCommand command) => 
    {
        var result = await mediator.Send(command, ct);

        if(result.IsFailure)
            return Results.BadRequest(result.Error);

        if(result.Value is UserResponseDto  createdUser)
            return Results.Created($"/users/{createdUser.Id}", createdUser);
        
        return Results.InternalServerError();
    });

    userRoute.MapPut("/", async (IMediator mediator, CancellationToken ct, [FromBody]UpdateUserCommand command) => 
    {
        var result = await mediator.Send(command, ct);
        
        if(result.IsFailure)
            return Results.BadRequest(result.Error);    
        
        if(result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.InternalServerError();
    });

    userRoute.MapPut("/{id:uint}", async (IMediator mediator, CancellationToken ct, uint id, [FromBody]UpdateUserCommand command) => 
    {
        if(!command.Id.Equals(id))
            return Results.BadRequest("Conflict of ID values");

        var result = await mediator.Send(command, ct);
        
        if(result.IsFailure)
            return Results.BadRequest(result.Error);    
        
        if(result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.InternalServerError();
    });

    userRoute.MapDelete("/{id:uint}", async (IMediator mediator, uint id, CancellationToken ct) => 
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, ct);
        
        if(result.IsFailure)
            return Results.BadRequest(result.Error);
        
        if(result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.InternalServerError();
    });

});

app.Run();