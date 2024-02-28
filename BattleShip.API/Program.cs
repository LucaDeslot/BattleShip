using BattleShip.API.services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
Dictionary<Guid, IGameService> gameServices = [];

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IGameService, GameService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/start/{difficulty}", (
        [FromRoute] int difficulty,
        IGameService IGameService) =>
{
    GameService gameService = (GameService) IGameService;
    gameServices.Add(gameService.GameId, gameService);
    List<Ship> grid = gameService.GridGeneration(difficulty);
    return new { id = gameService.GameId, Ships = grid, gridSize = gameService.GetGridSize()};
})
.WithOpenApi();

app.MapGet("/attack/{id}/{x}/{y}", ( //TODO:  handle id
    [FromRoute] Guid id,
    [FromRoute] int x,
    [FromRoute] int y,
    IGameService IGameService) =>
{
    GameService gameService = (GameService) IGameService;
    AttackResult result = gameServices[id].Attack(x, y);
    return result;
}).WithOpenApi();

app.MapGet("/game/{id}", (
    [FromRoute] Guid id) =>
{
    try
    {
        return new { id = id, Ships = gameServices[id].GetShips(), gridSize = gameServices[id].GetGridSize() };
    }
    catch (Exception e)
    {
        return null; 
    }
}).WithOpenApi();

app.MapGet("/history/{id}", (
    [FromRoute] Guid id) =>
{
    try
    {
        return gameServices[id].GetMoveHistories();
    }
    catch (Exception e)
    {
        return null;
    }
}).WithOpenApi();

app.Run();
