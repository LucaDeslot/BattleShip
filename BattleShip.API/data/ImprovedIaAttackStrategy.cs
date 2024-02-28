namespace BattleShip.API.data;

public class ImprovedIaAttackStrategy
{
    private int _isHorizontal = -1;
    private int _difficulty;
    private List<Coordinates> _aiAttacks = new List<Coordinates>(); // Liste des attaques possible niveau 1 à 2
    
    private List<Coordinates> _rightHorizontalAttacks = new List<Coordinates>(); // Liste des attaques horizontales possible vers le haut niveau 3
    private List<Coordinates> _leftHorizontalAttacks = new List<Coordinates>(); // Liste des attaques horizontales possible vers le bas niveau 3
    private List<Coordinates> _upperVerticalAttacks = new List<Coordinates>(); // Liste des attaques verticales possible vers le haut niveau 3
    private List<Coordinates> _lowerVerticalAttacks = new List<Coordinates>(); // Liste des attaques verticales possible vers le bas niveau 3
    private int position = 0;
    private char shipHit;
    
    public ImprovedIaAttackStrategy(int x, int y, char[,] playerGrid, int gridSize, int difficulty)
    {
        _difficulty = difficulty;
        shipHit = playerGrid[x, y];
        if (difficulty == 3) GenerateStrategyLevel3(x, y, playerGrid, gridSize);
        else GenerateStrategyLevel1And2(x, y, playerGrid, gridSize);
    }
    
    private void GenerateStrategyLevel3(int row, int col, char[,] playerGrid, int gridSize)
    {
        // Générer des attaques vertical
        for (var upperRow = 1; upperRow <= 4; upperRow++)
        {
            if(row - upperRow >= 0)
            {
                if (playerGrid[row - upperRow, col] != 'M' && playerGrid[row - upperRow, col] != 'H')
                    _upperVerticalAttacks.Add(new Coordinates { X = row - upperRow, Y = col });
            }
        }
        
        for (var lowerRow = 1; lowerRow <= 4; lowerRow++)
        {
            if(row + lowerRow < gridSize)
            {
                if (playerGrid[row + lowerRow, col] != 'M' && playerGrid[row + lowerRow, col] != 'H')
                    _lowerVerticalAttacks.Add(new Coordinates { X = row + lowerRow, Y = col });
            }
        }
        
        // Générer des attaques horizontales
        for (var rightCol = 1; rightCol <= 4; rightCol++)
        {
            if(col + rightCol < gridSize)
            {
                if (playerGrid[row, col + rightCol] != 'M' && playerGrid[row, col + rightCol] != 'H')
                    _rightHorizontalAttacks.Add(new Coordinates { X = row, Y = col + rightCol });
            }
        }
        
        for (var leftCol = 1; leftCol <= 4; leftCol++)
        {
            if(col - leftCol >= 0)
            {
                if (playerGrid[row, col - leftCol] != 'M' && playerGrid[row, col - leftCol] != 'H')
                    _leftHorizontalAttacks.Add(new Coordinates { X = row, Y = col - leftCol });
            }
        }
        Console.Write("Right: ");
    }

    private void GenerateStrategyLevel1And2(int x, int y, char[,] playerGrid, int gridSize)
    {
        // Générer des attaques à gauche et à droite de la cible
        for (var dx = -4; dx <= 4; dx++)
        {
            if (dx != 0) // Exclut la cellule centrale
            {
                if(x + dx >= 0 && x + dx < gridSize)
                    if (playerGrid[x + dx, y] != 'M' && playerGrid[x + dx, y] != 'H')
                        _aiAttacks.Add(new Coordinates { X = x + dx, Y = y });
            }
        }

        // Générer des attaques au-dessus et en dessous de la cible
        for (var dy = -4; dy <= 4; dy++)
        {
            if (dy != 0) // Exclut la cellule centrale
            {
                if(y + dy >= 0 && y + dy < gridSize)
                    if (playerGrid[x, y + dy] != 'M' && playerGrid[x, y + dy] != 'H')
                        _aiAttacks.Add(new Coordinates { X = x, Y = y + dy });
            }
        }
    }
    
    private bool IsFinished()
    {
        if (_difficulty == 3)
        {
            return _rightHorizontalAttacks.Count + _leftHorizontalAttacks.Count + _upperVerticalAttacks.Count + _lowerVerticalAttacks.Count == 0;
        }  
        return _aiAttacks.Count == 0;
    }

    public Coordinates? GetNextAttack()
    {
        if (IsFinished())
            return null;
        
        if (_difficulty == 3)
            return GetNextAttackLevel3();
        
        return GetNextAttackLevel1And2();
    }
    
    public void Hit(int row, int col) // Affinage de la stratégie
    {
        if (_difficulty == 3)
        {
            if ((_rightHorizontalAttacks.Count > 0 && row == _rightHorizontalAttacks[0].X) || (_leftHorizontalAttacks.Count > 0 && row == _leftHorizontalAttacks[0].X))
            {
                _isHorizontal = 1;
                _upperVerticalAttacks.Clear();
                _lowerVerticalAttacks.Clear();
            }
            else
            {
                _isHorizontal = 0;
                _rightHorizontalAttacks.Clear();
                _leftHorizontalAttacks.Clear();
            }
    }
        
        
    }
    
    private Coordinates? GetNextAttackLevel3()
    {
        Coordinates? nextAttack = null;

        while (nextAttack == null)
        {
            switch (position)
            {
                case 0:
                    if (_upperVerticalAttacks.Count > 0)
                    {
                        nextAttack = _upperVerticalAttacks[0];
                        _upperVerticalAttacks.RemoveAt(0);
                    }
                    position++;
                    break;
                case 1:
                    if(_rightHorizontalAttacks.Count > 0)
                    {
                        nextAttack = _rightHorizontalAttacks[0];
                        _rightHorizontalAttacks.RemoveAt(0);
                    }
                    position++;
                    break;
                case 2:
                    if(_lowerVerticalAttacks.Count > 0)
                    {
                        nextAttack = _lowerVerticalAttacks[0];
                        _lowerVerticalAttacks.RemoveAt(0);
                    }
                    position++;
                    break;
                case 3:
                    if(_leftHorizontalAttacks.Count > 0)
                    {
                        nextAttack = _leftHorizontalAttacks[0];
                        _leftHorizontalAttacks.RemoveAt(0);
                    }
                    position = 0;
                    break;
            }
        }
        return nextAttack;
    }

    private Coordinates GetNextAttackLevel1And2()
    {
        Coordinates nextAttack = _aiAttacks[0];
        _aiAttacks.RemoveAt(0);
        return nextAttack;
    }
    
    public char GetShipHit()
    {
        return shipHit;
    } 
}