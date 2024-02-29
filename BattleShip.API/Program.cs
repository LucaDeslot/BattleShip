using System.ComponentModel.DataAnnotations;
using BattleShip.API.data;
using BattleShip.API.services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;

Dictionary<Guid, IGameService> gameServices = [];
List<PlayerScore> leaderboard = [];

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();
builder.Services.AddSingleton<GameServiceRegistry>();
builder.Services.AddTransient<IValidator<AttackRequest>, AttackRequestValidator>();




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
        return new { id = id, Ships = gameServiceRegistry.GetGameService(id.ToString()).GetInitialPlayerShips(), gridSize = gameServiceRegistry.GetGameService(id.ToString()).GetGridSize() };

app.MapGet("/start/{difficulty}/{username}", (
        [FromRoute] int difficulty,
        [FromRoute] string username,
        IGameService IGameService) =>
    {
        bool userExistsInLeaderboard = leaderboard.Any(player => player.PlayerName == username);
        
        if (!userExistsInLeaderboard)
        {
            leaderboard.Add(new PlayerScore { PlayerName = username, VictoryNumber = 0 });
        }

        GameService gameService = (GameService) IGameService;
        gameServices.Add(gameService.GameId, gameService);
        List<Ship> grid = gameService.GridGeneration(difficulty);
        return new { id = gameService.GameId, Ships = grid, gridSize = gameService.GetGridSize()};
    })
    .WithOpenApi();


app.MapGet("/attack/{id}/{x}/{y}/{username}", async (
    [FromRoute] Guid id,
    [FromRoute] int x,
    [FromRoute] int y,
    [FromRoute] string username,
    IValidator<AttackRequest> validator) =>
{
    var request = new AttackRequest { Id = id, X = x, Y = y, GridSize = gameServices[id].GetGridSize()};
    ValidationResult validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.ToDictionary());
    }
    AttackResult resultAttack = gameServices[id].Attack(x, y);
    
    int index = leaderboard.FindIndex(p => p.PlayerName == username);
    if (index != -1 && resultAttack.Winner == "Player")
    {
        leaderboard[index].VictoryNumber++;
    }

    return Results.Ok(resultAttack);
}).WithOpenApi();



app.MapGet("/game/{id}", (
    [FromRoute] Guid id) =>
{
    try
    {
        return new { id = id, Ships = gameServices[id].GetInitialPlayerShips(), gridSize = gameServices[id].GetGridSize() };
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


app.MapGet("/leaderboard", () =>
{
    return leaderboard;
}).WithOpenApi();


app.Run();