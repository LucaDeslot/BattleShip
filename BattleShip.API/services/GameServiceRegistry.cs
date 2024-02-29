namespace BattleShip.API.services;

public class GameServiceRegistry
{
    private readonly Dictionary<Guid, IGameService> _gameServices = new();

    public void AddGameService(Guid id, IGameService gameService)
    {
        _gameServices[id] = gameService;
    }

    public IGameService GetGameService(Guid id)
    {
        _gameServices.TryGetValue(id, out var service);
        return service;
    }
}