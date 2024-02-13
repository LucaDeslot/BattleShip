namespace BattleShip.API.data;

public class ImprovedIaAttackStrategy
{
    private int _isHorizontal = -1;
    
    private List<Coordinates> _aiAttacks = new List<Coordinates>();
    
    public ImprovedIaAttackStrategy(int x, int y, char[,] playerGrid)
    {
        // Générer des attaques à gauche et à droite de la cible
        for (var dx = -4; dx <= 4; dx++)
        {
            if (dx != 0) // Exclut la cellule centrale
            {
                if(x + dx >= 0 && x + dx < 10)
                    if (playerGrid[x + dx, y] != 'M' && playerGrid[x + dx, y] != 'H')
                        _aiAttacks.Add(new Coordinates { X = x + dx, Y = y });
            }
        }

        // Générer des attaques au-dessus et en dessous de la cible
        for (var dy = -4; dy <= 4; dy++)
        {
            if (dy != 0) // Exclut la cellule centrale
            {
                if(y + dy >= 0 && y + dy < 10)
                    if (playerGrid[x, y + dy] != 'M' && playerGrid[x, y + dy] != 'H')
                        _aiAttacks.Add(new Coordinates { X = x, Y = y + dy });
            }
        }
    }

    public Coordinates? GetNextAttack()
    {
        if (_aiAttacks.Count == 0)
            return null;
        
        Coordinates nextAttack = _aiAttacks[0];
        _aiAttacks.RemoveAt(0);
        return nextAttack;
    }
}