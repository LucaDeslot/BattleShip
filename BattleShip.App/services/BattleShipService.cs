using System.Net.Http.Json;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using BattleShip.API.data;

public class BattleShipService {
    private readonly HttpClient _httpClient;

    public BattleShipService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public Task<StartGameResponse> StartGame(int difficulty, String username) {
        var response = _httpClient.GetFromJsonAsync<StartGameResponse>($"start/{difficulty}/{username}");
        return response;
    }

    public Task<StartGameResponse> GetGame(string gameId) {
        var response = _httpClient.GetFromJsonAsync<StartGameResponse>($"game/{gameId}");
        return response;
    }

    public Task<AttackResult> Attack(string gameId, int x, int y, string username) {
        Console.WriteLine($"attack/{gameId}/{x}/{y}/{username}");
        var response = _httpClient.GetFromJsonAsync<AttackResult>($"attack/{gameId}/{x}/{y}/{username}");
        return response;
    }
    
    public async Task<List<MoveHistory>> GetMoveHistory(string gameId) { //TODO: adapter les autres méthodes comme ça
        List<MoveHistory> response = await _httpClient.GetFromJsonAsync<List<MoveHistory>>($"history/{gameId}") ?? new ();
        return response;
    }

    public async Task<List<PlayerScore>> GetLeaderboard()
    {
        List<PlayerScore> response = await _httpClient.GetFromJsonAsync<List<PlayerScore>>("/leaderboard") ?? new();
        return response;
    }
}