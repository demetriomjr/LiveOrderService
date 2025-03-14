using LiveOrderService.Application.Authentication;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Application.Users;
using LiveOrderService.Common.Extensions;
using LiveOrderService.Infrastructure.Repositories;
using LiveOrderService.src.Application.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddDockerConfiguration();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IAuthRepository, AuthRepository>();

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseHttpsRedirection();

app.MapGroup("/user", userRoute =>
{
    userRoute.MapGet("/", async (IMediator mediator, CancellationToken ct) => 
    {
        var query = new GetAllUsersQuery();
        var result = await mediator.Send(query, ct);

        result.Match(
            users => Results.Ok(users),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    userRoute.MapGet("/{id:uint}", async (IMediator mediator, uint id, CancellationToken ct) => 
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, ct);

        result.Match(
            user => Results.Ok(user),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    userRoute.MapGet("/{username:string}", async (IMediator mediator, string username, CancellationToken ct) => 
    {
        var query = new GetUserByUsernameQuery(username);
        var result = await mediator.Send(query, ct);

        result.Match(
            user => Results.Ok(user),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    userRoute.MapPost("/", async (IMediator mediator, CancellationToken ct, [FromBody]CreateUserCommand command) => 
    {
        var result = await mediator.Send(command, ct);

        result.Match(
            user => Results.Created($"/user/{user.Id}", user),
            error => Results.BadRequest(error)
        );
        
        return Results.InternalServerError();
    });

    userRoute.MapPut("/", async (IMediator mediator, CancellationToken ct, [FromBody]UpdateUserCommand command) => 
    {
        var result = await mediator.Send(command, ct);
        
        result.Match(
            user => Results.Ok(),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    userRoute.MapPut("/{id:uint}", async (IMediator mediator, CancellationToken ct, uint id, [FromBody]UpdateUserCommand command) => 
    {
        if(!command.Id.Equals(id))
            return Results.BadRequest("Conflict of ID values");

        var result = await mediator.Send(command, ct);
        
        result.Match(
            user => Results.Ok(),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    userRoute.MapDelete("/{id:uint}", async (IMediator mediator, uint id, CancellationToken ct) => 
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, ct);
        
        result.Match(
            user => Results.NoContent(),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

});
app.MapGroup("/authentication", authRoute => 
{
    authRoute.MapGet("/validate_user", async (IMediator mediator, CancellationToken ct, 
    [AsParameters]string username, [AsParameters]string password, [AsParameters]string personalKey) => 
    {
        var query = new AuthenticateUserQuery(username, password, personalKey);
        var result = await mediator.Send(query, ct);

        result.Match(
            authResponse => Results.Ok(authResponse),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    authRoute.MapGet("/validate_token", async (IMediator mediator, CancellationToken ct,
    [AsParameters]string token, [AsParameters]string personalKey) => 
    {
        var query = new ValidateTokenQuery(token, personalKey);
        var result = await mediator.Send(query, ct);

        result.Match(
            newToken => Results.Ok(newToken),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });

    authRoute.MapGet("/refresh_token", async (IMediator mediator, CancellationToken ct,
    [AsParameters]string token, [AsParameters]string personalKey) => 
    {
        var query = new RefreshTokenQuery(token, personalKey);
        var result = await mediator.Send(query, ct);

        result.Match(
            newToken => Results.Ok(newToken),
            error => Results.BadRequest(error)
        );

        return Results.InternalServerError();
    });
});

app.Run();