using Type = Snake.Tile.Type;

namespace Snake;

public class SnakeGame
{
    public enum State
    {
        Ready,
        Playing,
        Dead,
    }

    public enum Direction
    {
        Up,
        Left,
        Right,
        Down
    }

    public int TailLength { get; private set; } = 1;
    
    internal ushort Rows;
    internal ushort Columns;
    private TilesUpdated? _tilesUpdated;

    internal const int WallWidth = 2;

    public State CurrentState { get; private set; } = State.Ready;

    private State _oldState = State.Ready;

    private Direction _snakeDirection;
    private Direction _oldSnakeDirection;
    private bool _alreadyMoved;

    private Position _snakeHeadPos;

    private List<Position> _snakeTailPos = new List<Position>();

    private Position _snakeTailEndPos;

    private Position _fruitPos;

    private bool _eating = false;
    
    public Tile[][] Tiles { get; }

    public delegate void TilesUpdated((Tile tile, Position position)[] updatedTiles);

    private void UpdateTiles((Tile tile, Position position)[] updatedTiles)
    {
        _tilesUpdated?.Invoke(updatedTiles);
    }

    private void UpdateTile(Tile tile, Position position)
    {
        UpdateTiles(new [] { (tile, position) });
    }
    
    public SnakeGame(ushort rows, ushort columns, TilesUpdated? tilesUpdatedCallback = null)
    {
        Rows = rows;
        Columns = columns;
        _tilesUpdated = tilesUpdatedCallback;

        Tiles = new Tile[Rows][];
        
        // Initialize each row and each columns
        Tiles = Tiles.Select(_ => new Tile[Columns].Select(
                _ => new Tile(Type.Empty)).ToArray()
        ).ToArray();
        
        InitializeWalls(WallWidth);
    }

    private void ChangeFruitPosition()
    {
        do
        {
            int fruitRow = Random.Shared.Next(WallWidth, Rows - WallWidth);
            int fruitColumn = Random.Shared.Next(WallWidth, Columns - WallWidth);

            _fruitPos = (fruitRow, fruitColumn);
        } while (_fruitPos == _snakeHeadPos ||
                 _fruitPos == _snakeTailEndPos ||
                 _snakeTailPos.Contains(_fruitPos));
        
        this[_fruitPos].TileType = Type.Fruit;
    }

    public void StartGame()
    {
        CurrentState = State.Playing;
        _snakeHeadPos = ((ushort)(Rows / 2), (ushort)(Columns / 2));

        _snakeTailEndPos = (_snakeHeadPos.Row, (ushort)(_snakeHeadPos.Column + 1));

        _snakeDirection = Direction.Left;

        ChangeFruitPosition();

        this[_snakeHeadPos].TileType = Type.HeadLeft;
        this[_snakeTailEndPos].TileType = Type.TailEndRight;

        UpdateTiles(new[]
        {
            (this[_fruitPos], _fruitPos),
            (this[_snakeHeadPos], _snakeHeadPos),
            (this[_snakeTailEndPos], _snakeTailEndPos)
        });
    }

    // FIXME: Make input handling better and more reliable
    public void LeftPress()
    {
        if (!_alreadyMoved && _snakeDirection != Direction.Right)
        {
            _alreadyMoved = true;
            _snakeDirection = Direction.Left;
        }
    }
    
    public void RightPress()
    {
        if (!_alreadyMoved && _snakeDirection != Direction.Left)
        {
            _alreadyMoved = true;
            _snakeDirection = Direction.Right;
        }
    }
    
    public void UpPress()
    {
        if (!_alreadyMoved && _snakeDirection != Direction.Down)
        {
            _alreadyMoved = true;
            _snakeDirection = Direction.Up;
        }
    }
    
    public void DownPress()
    {
        if (!_alreadyMoved && _snakeDirection != Direction.Up)
        {
            _alreadyMoved = true;   
            _snakeDirection = Direction.Down;
        }
    }

    public (Tile, Position)[] Update()
    {
        (Tile, Position)[] updatedTiles = UpdateGame();
        _oldState = CurrentState;
        _oldSnakeDirection = _snakeDirection;
        _alreadyMoved = false;
        return updatedTiles;
    }
    
    // FIXME: Update comment and make method simpler
    // This is where the game logic happens, this is optimized to only have to update 4 / 5 tiles at most
    // These tiles being: 
    //     - The snake head
    //     - The tail just after the snake head
    //     - The tail end (if we're not eating)
    //     - The fruit (if we're eating)
    private (Tile, Position)[] UpdateGame()
    {
        (Tile, Position)[] updatedTiles = new (Tile, Position)[6];
        updatedTiles[0] = (this[_snakeTailEndPos], _snakeTailEndPos);
        
        if (CurrentState != State.Playing)
            return Array.Empty<(Tile, Position)>();
        
        if(!_eating)
            this[_snakeTailEndPos].TileType = Type.Empty;

        // When we start the game, the snake doesn't have a tail yet
        if (_snakeTailPos.Count == 0)
        {
            this[_snakeHeadPos].TileType = _snakeDirection switch
            {
                Direction.Up => Type.TailEndDown,
                Direction.Left => Type.TailEndRight,
                Direction.Right => Type.TailEndLeft,
                Direction.Down => Type.TailEndUp,
                _ => throw new ArgumentOutOfRangeException()
            };
            _snakeTailEndPos = _snakeHeadPos;
        }
        else
        {
            if (_snakeDirection == Direction.Down && _oldSnakeDirection == Direction.Down)
                this[_snakeHeadPos].TileType = Type.TailUpDown;
            else if (_snakeDirection == Direction.Up && _oldSnakeDirection == Direction.Up)
                this[_snakeHeadPos].TileType = Type.TailUpUp;
            else if (_snakeDirection == Direction.Right && _oldSnakeDirection == Direction.Right)
                this[_snakeHeadPos].TileType = Type.TailLeftRight;
            else if (_snakeDirection == Direction.Left && _oldSnakeDirection == Direction.Left)
                this[_snakeHeadPos].TileType = Type.TailLeftLeft;
            else if (_snakeDirection == Direction.Right && _oldSnakeDirection == Direction.Down)
                this[_snakeHeadPos].TileType = Type.TailRightUp;
            else if (_snakeDirection == Direction.Up && _oldSnakeDirection == Direction.Left)
                this[_snakeHeadPos].TileType = Type.TailUpRight;
            else if (_snakeDirection == Direction.Left && _oldSnakeDirection == Direction.Down)
                this[_snakeHeadPos].TileType = Type.TailLeftUp;
            else if (_snakeDirection == Direction.Up && _oldSnakeDirection == Direction.Right)
                this[_snakeHeadPos].TileType = Type.TailUpLeft;
            else if (_snakeDirection == Direction.Left && _oldSnakeDirection == Direction.Up)
                this[_snakeHeadPos].TileType = Type.TailDownLeft;
            else if (_snakeDirection == Direction.Down && _oldSnakeDirection == Direction.Right)
                this[_snakeHeadPos].TileType = Type.TailLeftDown;
            else if (_snakeDirection == Direction.Right && _oldSnakeDirection == Direction.Up)
                this[_snakeHeadPos].TileType = Type.TailDownRight;
            else if (_snakeDirection == Direction.Down && _oldSnakeDirection == Direction.Left)
                this[_snakeHeadPos].TileType = Type.TailRightDown;
            else
                this[_snakeHeadPos].TileType = Type.Empty;

            Type oldTailType = Type.Empty;

            if (_eating)
                oldTailType = this[_snakeTailEndPos].TileType;
            
            _snakeTailEndPos = _snakeTailPos[^1];
            
            if (!_eating)
                oldTailType = this[_snakeTailEndPos].TileType;
            
            _snakeTailPos.RemoveAt(_snakeTailPos.Count - 1);
            _snakeTailPos.Insert(0, _snakeHeadPos);

            if (oldTailType == Type.Empty)
            {
                this[_snakeTailEndPos].TileType = _snakeDirection switch
                {
                    Direction.Down => Type.TailEndUp,
                    Direction.Up => Type.TailEndDown,
                    Direction.Left => Type.TailEndRight,
                    Direction.Right => Type.TailEndLeft,
                };
            }
            else
            {
                this[_snakeTailEndPos].TileType = oldTailType switch
                {
                    Type.TailUpDown => Type.TailEndUp,
                    Type.TailUpUp => Type.TailEndDown,
                    Type.TailLeftRight => Type.TailEndLeft,
                    Type.TailLeftLeft => Type.TailEndRight,
                    Type.TailUpRight => Type.TailEndDown,
                    Type.TailRightUp => Type.TailEndLeft,
                    Type.TailUpLeft => Type.TailEndDown,
                    Type.TailLeftUp => Type.TailEndRight,
                    Type.TailLeftDown => Type.TailEndUp,
                    Type.TailDownLeft => Type.TailEndRight,
                    Type.TailRightDown => Type.TailEndUp,
                    Type.TailDownRight => Type.TailEndLeft,
                    Type.TailEndDown => Type.TailEndDown,
                    Type.TailEndRight => Type.TailEndRight,
                    Type.TailEndUp => Type.TailEndUp,
                    Type.TailEndLeft => Type.TailEndLeft,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        updatedTiles[1] = (this[_snakeHeadPos], _snakeHeadPos);
        updatedTiles[2] = (this[_snakeTailEndPos], _snakeTailEndPos);

        // Move the head of the snake
        _snakeHeadPos += _snakeDirection switch
        {
            Direction.Left => (0, -1),
            Direction.Right => (0, 1),
            Direction.Up => (-1, 0),
            Direction.Down => (1, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        // Since we've moved, we can set the tile at the head position to be the head
        this[_snakeHeadPos].TileType = _snakeDirection switch
        {
            Direction.Up => Type.HeadUp,
            Direction.Left => Type.HeadLeft,
            Direction.Right => Type.HeadRight,
            Direction.Down => Type.HeadDown,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        updatedTiles[3] = (this[_snakeHeadPos], _snakeHeadPos);
        
        updatedTiles[4] = (this[_fruitPos], _fruitPos);

        if (_eating)
            _eating = false;
        // If we touch the fruit, add a tail from the end of the snake
        // and change the position of the fruit
        if (_snakeHeadPos == _fruitPos)
        {
            _snakeTailPos.Add(_snakeTailEndPos);
            ChangeFruitPosition();
            _eating = true;
        }
        updatedTiles[5] = (this[_fruitPos], _fruitPos);

        if (_snakeHeadPos.Column == WallWidth - 1 || // Are we touching the left wall?
            _snakeHeadPos.Column == Columns - WallWidth || // Are we touching the right wall?
            _snakeHeadPos.Row == WallWidth - 1 || // Are we touching the top wall?
            _snakeHeadPos.Row == Rows - WallWidth) // Are we touching the bottom wall?
        {
            CurrentState = State.Dead;
        }

        // Are we touching our own tail
        if (_snakeHeadPos == _snakeTailEndPos || _snakeTailPos.Contains(_snakeHeadPos))
        {
            CurrentState = State.Dead;
        }

        return updatedTiles;
    }

    private void InitializeWalls(ushort wallWidth)
    {
        for (ushort i = 0; i < wallWidth; i++)
        {
            InitializeRow(i, Type.WallTop);
            InitializeRow((ushort)(Rows - i - 1), Type.WallBottom);
            
            InitializeColumn(i, Type.WallLeft);
            InitializeColumn((ushort)(Columns - i - 1), Type.WallRight);
        }
    }

    private void InitializeRow(ushort row, Type type)
    {
        foreach (Tile tile in Tiles[row])
            tile.TileType = type;
    }
    
    private void InitializeColumn(ushort column, Type type)
    {
        foreach (Tile[] tile in Tiles)
            tile[column].TileType = type;
    }

    public Tile this[(ushort row, ushort column) position] => Tiles[position.row][position.column];
    
    public Tile this[Position position] => Tiles[position.Row][position.Column];
}