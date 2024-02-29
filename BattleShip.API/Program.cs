using BattleShip.API.services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();
builder.Services.AddSingleton<GameServiceRegistry>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:9000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseCors();

app.MapGrpcService<GameServiceGrpcImpl>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/start/{difficulty}", (
        [FromRoute] int difficulty,
        GameServiceRegistry gameServiceRegistry) =>
{
    GameService gameService = new GameService();
    gameServiceRegistry.AddGameService(gameService.GameId, gameService);
    List<Ship> grid = gameService.GridGeneration(difficulty);
    return new { id = gameService.GameId, Ships = grid, gridSize = gameService.GetGridSize()};
})
.WithOpenApi();

app.MapGet("/attack/{id}/{x}/{y}", ( //TODO:  handle id
    [FromRoute] Guid id,
    [FromRoute] int x,
    [FromRoute] int y,
    GameServiceRegistry gameServiceRegistry) =>
{
    AttackResult result = gameServiceRegistry.GetGameService(id).Attack(x, y);
    return result;
}).WithOpenApi();

app.MapGet("/game/{id}", (
    [FromRoute] Guid id,
    GameServiceRegistry gameServiceRegistry) =>
{
    try
    {
        return new { id = id, Ships = gameServiceRegistry.GetGameService(id).GetInitialPlayerShips(), gridSize = gameServiceRegistry.GetGameService(id).GetGridSize() };
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
        return gameServiceRegistry.GetGameService(id).GetMoveHistories();
    }
    catch (Exception e)
    {
        return null;
    }
}).WithOpenApi();

app.Run();
