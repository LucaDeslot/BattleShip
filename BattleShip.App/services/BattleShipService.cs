using System.Net.Http.Json;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using BattleShip.API.data;

public class BattleShipService {
    private readonly HttpClient _httpClient;

    public BattleShipService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public Task<StartGameResponse> StartGame(int difficulty) {
        var response = _httpClient.GetFromJsonAsync<StartGameResponse>($"start/{difficulty}");
        return response;
    }

    public Task<StartGameResponse> GetGame(string gameId) {
        var response = _httpClient.GetFromJsonAsync<StartGameResponse>($"game/{gameId}");
        return response;
    }

    public Task<AttackResult> Attack(string gameId, int x, int y) {
        var response = _httpClient.GetFromJsonAsync<AttackResult>($"attack/{gameId}/{x}/{y}");
        return response;
    }
    
    public async Task<List<MoveHistory>> GetMoveHistory(string gameId) { //TODO: adapter les autres méthodes comme ça
        List<MoveHistory> response = await _httpClient.GetFromJsonAsync<List<MoveHistory>>($"history/{gameId}") ?? new ();
        return response;
    } 
}