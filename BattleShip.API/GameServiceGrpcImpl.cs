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
            GameId = gameService.GameId.ToString(),
            Ships = {ConvertShipsToProto(grid)},
            GridSize = gameService.GetGridSize()
        };
        return Task.FromResult(response);
    }

    public override Task<AttackResultGrpc> Attack(AttackRequest request, ServerCallContext context)
    {
        IGameService gameService = _gameServiceRegistry.GetGameService(request.GameId);
        AttackResult attackResult = gameService.Attack(request.Row, request.Col);
        CoordinatesGrpc coordinates = new CoordinatesGrpc
        {
            Row = attackResult.IACoordinates.X,
            Col = attackResult.IACoordinates.Y
        };
        var result = new AttackResultGrpc {
            PlayerAttackResult = attackResult.PlayerAttackResult.ToString(),
            Coordinates = coordinates,
            IaAttackResult = attackResult.IAAttackResult.ToString(),
            Winner = attackResult.Winner
        };
        return Task.FromResult(result);
    }

    private RepeatedField<ShipGrpc> ConvertShipsToProto(List<Ship> ships)
    {
        var protoShips = new RepeatedField<ShipGrpc>();
        foreach (var ship in ships)
        {
            protoShips.Add(ConvertShipToProto(ship));
        }
        return protoShips;
    }
    
    private ShipGrpc ConvertShipToProto(Ship ship)
    {
        return new ShipGrpc
        {
            Type = ship.Type.ToString(),
            StartRow = ship.StartRow,
            StartCol = ship.StartCol,
            Size = ship.Size,
            IsHorizontal = ship.IsHorizontal
        };
    }
}