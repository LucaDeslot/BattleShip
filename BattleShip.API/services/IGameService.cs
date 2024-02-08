public interface IGameService
{

    List<Ship> GridGeneration();

    AttackResult Attack(int x, int y);

    List<Ship> GetShips();

}
