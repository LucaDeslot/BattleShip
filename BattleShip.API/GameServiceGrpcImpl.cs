using Google.Protobuf.Collections;
using Grpc.Core;

namespace BattleShip.API.services;

public class GameServiceGrpcImpl : BattleShipService.BattleShipServiceBase
{

    private readonly GameServiceRegistry _gameServiceRegistry;
    
    public GameServiceGrpcImpl(GameServiceRegistry gameServiceRegistry)
    {
        _gameServiceRegistry = gameServiceRegistry;
    }
    
    public override Task<StartGameResponse> StartGame(StartGameRequest request, ServerCallContext context)
    {
        GameService gameService = new GameService();
        _gameServiceRegistry.AddGameService(gameService.GameId, gameService);
        List<Ship> grid = gameService.GridGeneration(request.Difficulty);
        
        var response = new StartGameResponse
        {
            GameId = Guid.NewGuid().ToString(),
            Ships = {ConvertShipsToProto(grid)},
            GridSize = gameService.GetGridSize()
        };
        return Task.FromResult(response);
    }
    
    private RepeatedField<ShipGRPC> ConvertShipsToProto(List<Ship> ships)
    {
        var protoShips = new RepeatedField<ShipGRPC>();
        foreach (var ship in ships)
        {
            protoShips.Add(ConvertShipToProto(ship));
        }
        return protoShips;
    }
    
    private ShipGRPC ConvertShipToProto(Ship ship)
    {
        return new ShipGRPC
        {
            Type = ship.Type.ToString(),
            StartRow = ship.StartRow,
            StartCol = ship.StartCol,
            Size = ship.Size,
            IsHorizontal = ship.IsHorizontal
        };
    }
}