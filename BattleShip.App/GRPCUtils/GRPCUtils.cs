using Google.Protobuf.Collections;

public class GRPCUtils
{
    public static List<Ship> DeserializeShips(RepeatedField<ShipGrpc> response)
    {
        List<Ship> ships = new();
        foreach (var shipGrpc in response)
        {
            Ship ship = new();
            ship.fromGrpc(shipGrpc);
            ships.Add(ship);
        }
        return ships;
    }
    
    public static AttackRequestGrpc SerializeAttackRequest(string gameId, int x, int y, string username)
    {
        return new AttackRequestGrpc
        {
            GameId = gameId,
            Row = x,
            Col = y,
            Username = username
        };
    }
    
    public static StartGameResponse DeserializeStartGameResponse(StartGameResponseGrpc response)
    {
        List<Ship> ships = DeserializeShips(response.Ships);
        return new StartGameResponse(Guid.Parse(response.GameId), ships, response.GridSize);
    }
    
    public static AttackResult DeserializeAttackResult(AttackResultGrpc response)
    {
        return new AttackResult(response.PlayerAttackResult[0], DeserializeCoordinates(response.Coordinates), response.IaAttackResult[0], response.Winner);
    }
    
    public static Coordinates DeserializeCoordinates(CoordinatesGrpc coordinates)
    {
        return new Coordinates{X = coordinates.Row, Y = coordinates.Col};
    }
}