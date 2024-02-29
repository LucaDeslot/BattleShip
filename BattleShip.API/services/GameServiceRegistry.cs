using BattleShip.API.data;

namespace BattleShip.API.services;

public class GameServiceRegistry
{
    private readonly Dictionary<Guid, IGameService> _gameServices = new();
    List<PlayerScore> leaderboard = [];


    public void AddGameService(Guid id, IGameService gameService)
    {
        _gameServices[id] = gameService;
    }

    public IGameService GetGameService(string id)
    {
        _gameServices.TryGetValue(Guid.Parse(id), out var service);
        return service;
    }
    
    public List<PlayerScore> GetLeaderboard()
    {
        return leaderboard;
    }
    
    public bool UserExistsInLeaderboard(string username)
    {
        return leaderboard.Any(player => player.PlayerName == username);
    }
    
    public void AddUserToLeaderboard(string username)
    {
        leaderboard.Add(new PlayerScore { PlayerName = username, VictoryNumber = 0 });
    }
    
    public void AddVictoryLeaderboard(string username)
    {
        int index = leaderboard.FindIndex(p => p.PlayerName == username);
        if (index != -1)
        {
            leaderboard[index].VictoryNumber++;
        }
    }
}