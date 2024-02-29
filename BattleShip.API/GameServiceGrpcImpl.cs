using FluentValidation;
using FluentValidation.Results;
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
    
    public override Task<StartGameResponseGrpc> StartGame(StartGameRequestGrpc request, ServerCallContext context)
    {
        InitLeaderboard(request.Username);
        GameService gameService = new GameService();
        _gameServiceRegistry.AddGameService(gameService.GameId, gameService);
        List<Ship> grid = gameService.GridGeneration(request.Difficulty);
        
        var response = new StartGameResponseGrpc
        {
            GameId = gameService.GameId.ToString(),
            Ships = {ConvertShipsToProto(grid)},
            GridSize = gameService.GetGridSize()
        };
        return Task.FromResult(response);
    }
    
    private void InitLeaderboard(string username)
    {

        bool userExistsInLeaderboard = _gameServiceRegistry.UserExistsInLeaderboard(username);
         
         if (!userExistsInLeaderboard)
         {
             _gameServiceRegistry.AddUserToLeaderboard(username);
         }
    }

    public override Task<AttackResultGrpc> Attack(AttackRequestGrpc request, ServerCallContext context)
    {
        // Validation
        var validator = new AttackRequestValidator();
        ValidationResult validationResult = validator.Validate(request);
        
        if (validationResult.IsValid)
        {
            var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessage));
        }
        
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
        
        if (attackResult.Winner == "Player")
        {
            _gameServiceRegistry.AddVictoryLeaderboard(request.Username);
        }
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
            CurrentSize = ship.CurrentSize,
            Size = ship.Size,
            IsHorizontal = ship.IsHorizontal
        };
    }
}