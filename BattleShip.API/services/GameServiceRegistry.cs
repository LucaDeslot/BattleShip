namespace BattleShip.API.services;

public class GameServiceRegistry
{
    private readonly Dictionary<Guid, IGameService> _gameServices = new();

    public void AddGameService(Guid id, IGameService gameService)
    {
        _gameServices[id] = gameService;
    }

    public IGameService GetGameService(string id)
    {
        _gameServices.TryGetValue(Guid.Parse(id), out var service);
        return service;
    }
}