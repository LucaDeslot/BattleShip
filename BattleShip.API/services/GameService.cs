using BattleShip.API.data;

namespace BattleShip.API.services;

public class GameService : IGameService
{
    private char[,] _playerGrid;
    private char[,] _iaGrid;

    private readonly List<Ship> _playerShips = [];
    private readonly List<Ship> _aiShips = [];
    
    private List<Coordinates> _aiAttacks = new List<Coordinates>();
    
    private bool _improvedIa = true;
    private ImprovedIaAttackStrategy _improvedIaAttackStrategy;

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
        InitShips(_aiShips, _iaGrid);
        InitShips(_playerShips, _playerGrid);
        InitAiAttack();
        return _playerShips;
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

    private void InitShips(List<Ship> ships, char[,] grid)
    {
        char[] shipTypes = { 'A', 'B', 'C', 'D', 'E' };
        // Initialize player ships
        foreach (var shipType in shipTypes)
        {
            switch (shipType)
            {
                case 'A':
                    ships.Add(PlaceShip(grid, shipType, 5));
                    break;
                case 'B':
                    ships.Add(PlaceShip(grid, shipType, 4));
                    break;
                case 'C':
                    ships.Add(PlaceShip(grid, shipType, 3));
                    break;
                case 'D':
                    ships.Add(PlaceShip(grid, shipType, 3));
                    break;
                case 'E':
                    ships.Add(PlaceShip(grid, shipType, 2));
                    break;
            }
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
        int y = _aiAttacks[0].Y;
        int x = _aiAttacks[0].X;
        if (_improvedIaAttackStrategy != null)
        {
            Coordinates? attack = _improvedIaAttackStrategy.GetNextAttack();
            if (attack != null)
            {
                x = attack.X;
                y = attack.Y;
                Coordinates? coordinatesToDelete = _aiAttacks.Find(coordinates => coordinates.X == x && coordinates.Y == y);
                if (coordinatesToDelete != null)
                {
                    _aiAttacks.Remove(coordinatesToDelete);
                }
            }
            else
            {
                _improvedIaAttackStrategy = null;
            }
        }
        else
        {
            _aiAttacks.RemoveAt(0);
        }
        
        char shipValue = _playerGrid[x, y];
        if(shipValue == '\0')
        {
            _playerGrid[x, y] = 'M';
            result.IAAttackResult = 'M';
            result.IACoordinates.X = x;
            result.IACoordinates.Y = y;
        } else if(shipValue != 'H') //Un bateau a été touché
        {
            if (_improvedIaAttackStrategy == null)
            {
                _improvedIaAttackStrategy = new ImprovedIaAttackStrategy(x, y, _playerGrid);
            }
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
                    _improvedIaAttackStrategy = null;
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