using BattleShip.API.data;

public interface IGameService
{

    List<Ship> GridGeneration(int difficulty);

    AttackResult Attack(int x, int y);

    List<Ship> GetShips();

    int GetGridSize();
    
    List<MoveHistory> GetMoveHistories();
}
