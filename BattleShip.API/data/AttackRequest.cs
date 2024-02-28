namespace BattleShip.API.data;

public class AttackRequest
{
    public Guid Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int GridSize { get; set; }
}
