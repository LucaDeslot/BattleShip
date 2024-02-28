using BattleShip.API.data;
using BattleShip.API.services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
Dictionary<Guid, IGameService> gameServices = [];

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

builder.Services.AddTransient<IValidator<AttackRequest>, AttackRequestValidator>();

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


app.MapGet("/attack/{id}/{x}/{y}", async (
    [FromRoute] Guid id,
    [FromRoute] int x,
    [FromRoute] int y,
    IValidator<AttackRequest> validator,
    IGameService IGameService) =>
{
    var request = new AttackRequest { Id = id, X = x, Y = y };
    
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return null;
    }
    GameService gameService = (GameService) IGameService;
    AttackResult result = gameServices[id].Attack(x, y);
    return result;

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
