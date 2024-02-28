namespace BattleShip.API.data;

public class MoveHistory
{
    public Guid GameId { get; set; }
    public int Turn { get; set; }
    public bool Player { get; set; }
    public int row { get; set; }
    public int col { get; set; }
    public char Result { get; set; }
    public int GridSize { get; set; }
    public char Type { get; set; }
    
    public MoveHistory(Guid gameId, int turn, bool player, int row, int col, char result, int gridSize)
    {
        GameId = gameId;
        Turn = turn;
        Player = player;
        this.row = row;
        this.col = col;
        Result = result;
        GridSize = gridSize;
    }
}