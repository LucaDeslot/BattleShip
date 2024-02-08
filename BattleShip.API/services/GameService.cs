namespace BattleShip.API.services;

public class GameService : IGameService
{
    private char[,] _playerGrid;
    private char[,] _iaGrid;

    private readonly List<Ship> _playerShips = [];
    private readonly List<Ship> _aiShips = [];
    
    private List<Coordinates> _aiAttacks = new List<Coordinates>();
    private int _aiAttackIndex = 0;

    public Guid GameId { get; }

    public GameService()
    {
        GameId = Guid.NewGuid();
    }

    public List<Ship> GridGeneration()
    {
        _playerGrid = new char[10, 10];
        _iaGrid = new char[10, 10];
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                _playerGrid[i, j] = '\0';
                _iaGrid[i, j] = '\0';
            }
        }
        InitShips();
        InitAiAttack();
        return this._playerShips;
    }

    private void InitAiAttack()
    {
        Random random = new Random();
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                _aiAttacks.Add(new Coordinates { X = i, Y = j });
            }
        }
        _aiAttacks = _aiAttacks.OrderBy(x => random.Next()).ToList();
    }

    private void InitShips()
    {
        Random random = new Random();
        char[] shipTypes = { 'A', 'B', 'C', 'D', 'E', 'F' };
        // Initialize player ships
        foreach (var shipType in shipTypes)
        {
            int shipSize = random.Next(1, 5);
            _playerShips.Add(PlaceShip(_playerGrid, shipType, shipSize));
        }

        // Initialize AI ships
        foreach (var shipTypeAi in shipTypes)
        {
            int shipSize = random.Next(1, 5);
            _aiShips.Add(PlaceShip(_iaGrid, shipTypeAi, shipSize));
        }
    }

    private Ship PlaceShip(char[,] grid, char shipType, int shipSize)
    {
        Random random = new Random();
        bool isPlaced = false;
        Ship ship = new Ship();

        while (!isPlaced)
        {
            int row = random.Next(0, 10);
            int col = random.Next(0, 10);
            bool isHorizontal = random.Next(0, 2) == 0;
            if (CanPlaceShip(grid, row, col, shipSize, isHorizontal))
            {
                for (int i = 0; i < shipSize; i++)
                {
                    if (isHorizontal)
                    {
                        grid[row, col + i] = shipType;
                        if (i == 0) {
                            ship.Type = shipType;
                            ship.StartRow = row;
                            ship.StartCol = col + i;
                            ship.Size = shipSize;
                            ship.IsHorizontal = true;
                        }
                    }
                    else
                    {
                        grid[row + i, col] = shipType;
                        if (i == 0) {
                            ship.Type = shipType;
                            ship.StartRow = row + i;
                            ship.StartCol = col;
                            ship.Size = shipSize;
                            ship.IsHorizontal = false;
                        }
                    }
                }
                isPlaced = true;
            }
        }
        return ship;
    }

    private bool CanPlaceShip(char[,] grid, int row, int col, int shipSize, bool isHorizontal)

    {
        if (isHorizontal)
        {
            if (col + shipSize > 10) // Dépassement de la grille
            {
                return false;
            }

            for (int i = 0; i < shipSize; i++)
            {
                if (grid[row, col + i] != '\0')
                {
                    return false;
                }
            }
        }
        else
        {
            if (row + shipSize > 10)
            {
                return false;
            }

            for (int i = 0; i < shipSize; i++)
            {
                if (grid[row + i, col] != '\0')
                {
                    return false;
                }
            }
        }
        return true;
    }

    public AttackResult Attack(int x, int y)
    {
        AttackResult result = new();
        char shipValue = _iaGrid[x, y];
        if(shipValue == '\0')
        {
            _iaGrid[x, y] = 'M';
            result.PlayerAttackResult = 'M';
        } else if(shipValue != 'H')
        {
            _iaGrid[x, y] = 'H';
            result.PlayerAttackResult = 'H';
            Ship? ship = _aiShips.Find(s => s.Type == shipValue);
            if(ship != null)
            {
                ship.Size--;
                if(ship.Size == 0)
                {
                    _aiShips.Remove(ship);
                }
            }
           
        }
        IaAttack(result);
        CheckWinner(result);
        return result;
    }
    private void IaAttack(AttackResult result)
    {
        int y = _aiAttacks[_aiAttackIndex].Y;
        int x = _aiAttacks[_aiAttackIndex].X;
        _aiAttackIndex++;
        
        char shipValue = _playerGrid[x, y];
        if(shipValue == '\0')
        {
            _playerGrid[x, y] = 'M';
            result.IAAttackResult = 'M';
            result.IACoordinates.X = x;
            result.IACoordinates.Y = y;
        } else if(shipValue != 'H')
        {
            _playerGrid[x, y] = 'H';
            result.IAAttackResult = 'H';
            result.IACoordinates.X = x;
            result.IACoordinates.Y = y;
            Ship? ship = _playerShips.Find(s => s.Type == shipValue);
            if(ship != null)
            {
                ship.Size--;
                if(ship.Size == 0)
                {
                    _playerShips.Remove(ship);
                }
            }
        }
    }

    public List<Ship> GetShips()
    {
        return _playerShips;
    }

    private void CheckWinner(AttackResult result)
    {
        if(_playerShips.Count == 0)
        {
            result.Winner = "IA";
        } else if(_aiShips.Count == 0)
        {
            result.Winner = "Player";
        }
    }
}