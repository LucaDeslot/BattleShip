using System.Net.Http.Json;
using BattleShip.API.data;

public class BattleShipServiceImpl {
    private readonly HttpClient _httpClient;

    public BattleShipServiceImpl(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public Task<StartGameResponse> GetGame(string gameId) {
        var response = _httpClient.GetFromJsonAsync<StartGameResponse>($"game/{gameId}");
        return response;
    }
    
    public async Task<List<MoveHistory>> GetMoveHistory(string gameId) { //TODO: adapter les autres méthodes comme ça
        List<MoveHistory> response = await _httpClient.GetFromJsonAsync<List<MoveHistory>>($"history/{gameId}") ?? new ();
        return response;
    } 
    
    public async Task<List<PlayerScore>> GetLeaderboard() {
        List<PlayerScore> response = await _httpClient.GetFromJsonAsync<List<PlayerScore>>("/leaderboard") ?? new ();
        return response;
    }
}