using BattleShip.API.services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();
builder.Services.AddSingleton<GameServiceRegistry>();
builder.Services.AddTransient<IValidator<AttackRequestGrpc>, AttackRequestValidator>();




builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:9000")
                .AllowAnyHeader()
                .AllowAnyMethod();
            builder.WithOrigins("https://localhost:9000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseGrpcWeb(); 

// Configure the HTTP request pipeline.
app.UseCors();

app.MapGrpcService<GameServiceGrpcImpl>()
    .EnableGrpcWeb()
    .RequireCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/game/{id}", (
    [FromRoute] Guid id,
    GameServiceRegistry gameServiceRegistry) =>
{
    try
    {
        return new
        {
            id = id, Ships = gameServiceRegistry.GetGameService(id.ToString()).GetInitialPlayerShips(),
            gridSize = gameServiceRegistry.GetGameService(id.ToString()).GetGridSize()
        };
    }
    catch (Exception e)
    {
        return null;
    }
}).WithOpenApi();

app.MapGet("/history/{id}", (
    [FromRoute] Guid id,
    GameServiceRegistry gameServiceRegistry) =>
{
    try
    {
        return gameServiceRegistry.GetGameService(id.ToString()).GetMoveHistories();
    }
    catch (Exception e)
    {
        return null;
    }
}).WithOpenApi();


app.MapGet("/leaderboard", (GameServiceRegistry gameServiceRegistry) =>
{
    return gameServiceRegistry.GetLeaderboard();
}).WithOpenApi();


app.Run();