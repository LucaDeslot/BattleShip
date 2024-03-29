using BattleShip.API.data;

namespace BattleShip.API.services;

public class GameService : IGameService
{
    private char[,] _playerGrid;
    private char[,] _iaGrid;

    private readonly List<Ship> _playerShips = [];
    private readonly List<Ship> _aiShips = [];
    private List<Ship> _InitialPlayerShips = [];
    
    private List<Coordinates> _aiAttacks = new List<Coordinates>();
    
    private bool _improvedIa = true;
    private ImprovedIaAttackStrategy _improvedIaAttackStrategy;
    
    private int _gridSize = 10;
    private int _difficulty;
    
    private DateTime _gameDate = DateTime.Now;
    
    private List<MoveHistory> _moveHistories = new List<MoveHistory>();

    public Guid GameId { get; }

    public GameService()
    {
        GameId = Guid.NewGuid();
    }

    public List<Ship> GridGeneration(int difficulty)
    {
        _difficulty = difficulty;
        
        if(difficulty > 1) _gridSize = 15;
        
        _playerGrid = new char[_gridSize, _gridSize];
        _iaGrid = new char[_gridSize, _gridSize];
        for (var i = 0; i < _gridSize; i++)
        {
            for (var j = 0; j < _gridSize; j++)
            {
                _playerGrid[i, j] = '\0';
                _iaGrid[i, j] = '\0';
            }
        }
        InitShips(_aiShips, _iaGrid);
        _InitialPlayerShips = new List<Ship>(InitShips(_playerShips, _playerGrid));
        InitAiAttack();
        return _playerShips;
    }

    private void InitAiAttack()
    {
        Random random = new Random();
        for (var i = 0; i < _gridSize; i++)
        {
            for (var j = 0; j < _gridSize; j++)
            {
                _aiAttacks.Add(new Coordinates { X = i, Y = j });
            }
        }
        _aiAttacks = _aiAttacks.OrderBy(x => random.Next()).ToList();
    }

    private List<Ship> InitShips(List<Ship> ships, char[,] grid)
    {
        char[] shipTypes = { 'A', 'B', 'C', 'D', 'E' };
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

        return ships;
    }

    private Ship PlaceShip(char[,] grid, char shipType, int shipSize)
    {
        Random random = new Random();
        bool isPlaced = false;
        Ship ship = new Ship();

        while (!isPlaced)
        {
            int row = random.Next(0, _gridSize -1);
            int col = random.Next(0, _gridSize -1);
            bool isHorizontal = random.Next(0, 2) == 1;
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
                            ship.CurrentSize = shipSize;
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
                            ship.CurrentSize = shipSize;
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
            if (col + shipSize > _gridSize - 1) // Dépassement de la grille
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
            if (row + shipSize > _gridSize - 1)
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
    
    public List<Ship> GetInitialPlayerShips()
    {
        return _InitialPlayerShips;
    }

    public AttackResult Attack(int x, int y)
    {
        AttackResult result = new();
        char shipValue = _iaGrid[x, y];
        if(shipValue == '\0')
        { // Aucun bateau n'a été touché
            RegisterHistory(x, y, true, 'M');
            _iaGrid[x, y] = 'M';
            result.PlayerAttackResult = 'M';
        } else if(shipValue != 'H')
        { // Un bateau a été touché
            RegisterHistory(x, y, true, 'H');
            _iaGrid[x, y] = 'H';
            result.PlayerAttackResult = 'H';
            Ship? ship = _aiShips.Find(s => s.Type == shipValue);
            if(ship != null)
            {
                ship.CurrentSize--;
                if(ship.CurrentSize == 0)
                {
                    _aiShips.Remove(ship);
                }
            }
           
        }
        IaAttack(result);
        CheckWinner(result);
        return result;
    }

    private void RegisterHistory(int x, int y, bool player, char result)
    {
        MoveHistory moveHistory = new MoveHistory(GameId, _moveHistories.Count, player, x, y, result, _gridSize);
        _moveHistories.Add(moveHistory);
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
            RegisterHistory(x, y, false, 'M');
            _playerGrid[x, y] = 'M';
            result.IAAttackResult = 'M';
            result.IACoordinates.X = x;
            result.IACoordinates.Y = y;
        } else if(shipValue != 'H') //Un bateau a été touché
        {
            if (_improvedIaAttackStrategy == null)
            {
                _improvedIaAttackStrategy = new ImprovedIaAttackStrategy(x, y, _playerGrid, _gridSize, _difficulty);
            } else {
                if(shipValue == _improvedIaAttackStrategy.GetShipHit())
                    _improvedIaAttackStrategy.Hit(x, y);
            }
            RegisterHistory(x, y, false, 'H');
            _playerGrid[x, y] = 'H';
            result.IAAttackResult = 'H';
            result.IACoordinates.X = x;
            result.IACoordinates.Y = y;
            Ship? ship = _playerShips.Find(s => s.Type == shipValue);
            if(ship != null)
            {
                ship.CurrentSize--;
                if(ship.CurrentSize == 0)
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
    
    public int GetGridSize()
    {
        return _gridSize;
    }
    
    public List<MoveHistory> GetMoveHistories()
    {
        return _moveHistories;
    }
    
    public DateTime GetGameDate()
    {
        return _gameDate;
    }
}