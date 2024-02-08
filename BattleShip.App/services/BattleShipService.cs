using System.Net.Http.Json;

using System.Collections.Generic;

public class BattleShipService {
    private readonly HttpClient _httpClient;

    public BattleShipService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public Task<StartGameResponse> StartGame() {
        var response = _httpClient.GetFromJsonAsync<StartGameResponse>("start");
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
}