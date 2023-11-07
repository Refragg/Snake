using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Snake;

public class GraphicalSnake
{
    private static SnakeGame _snakeGame;

    private const int TileSize = 32;
    private const int Columns = 25;
    private const int Rows = 25;
    
    private static Sprite[][] _tiles;

    private static Texture _backgroundTexture = new Texture(new Image(TileSize, TileSize, Color.Black));
    private static Texture _wallTexture = new Texture(new Image("Assets/wall.png"));
    private static Texture _fruitTexture = new Texture(new Image("Assets/fruit.png"));
    
    private static Texture _snakeHeadLeftTexture = new Texture(new Image("Assets/headleft.png"));
    private static Texture _snakeHeadRightTexture = new Texture(new Image("Assets/headright.png"));
    private static Texture _snakeHeadUpTexture = new Texture(new Image("Assets/headup.png"));
    private static Texture _snakeHeadDownTexture = new Texture(new Image("Assets/headdown.png"));
    
    private static Texture _snakeTailLeftRightTexture = new Texture(new Image("Assets/tailleftright.png"));
    private static Texture _snakeTailUpDownTexture = new Texture(new Image("Assets/tailupdown.png"));
    private static Texture _snakeTailUpRightTexture = new Texture(new Image("Assets/tailupright.png"));
    private static Texture _snakeTailUpLeftTexture = new Texture(new Image("Assets/tailupleft.png"));
    private static Texture _snakeTailDownRightTexture = new Texture(new Image("Assets/taildownright.png"));
    private static Texture _snakeTailDownLeftTexture = new Texture(new Image("Assets/taildownleft.png"));
    
    private static Texture _snakeTailEndUpTexture = new Texture(new Image("Assets/tailendup.png"));
    private static Texture _snakeTailEndDownTexture = new Texture(new Image("Assets/tailenddown.png"));
    private static Texture _snakeTailEndLeftTexture = new Texture(new Image("Assets/tailendleft.png"));
    private static Texture _snakeTailEndRightTexture = new Texture(new Image("Assets/tailendright.png"));
    
    public static void Snake(string[] args)
    {
        RenderWindow window = new RenderWindow(new VideoMode(800, 800), "Snake",  Styles.Close);
        window.Closed += (sender, eventArgs) => window.Close();
        window.KeyPressed += WindowOnKeyPressed;

        // Initialize each row and each columns
        _tiles = new Sprite[Rows][];

        for (int x = 0; x < Rows; x++)
        {
            _tiles[x] = new Sprite[Columns];
            for (int y = 0; y < _tiles[x].Length; y++)
            {
                Sprite sprite = new Sprite(_backgroundTexture)
                {
                    Position = new Vector2f(y * TileSize, x * TileSize),
                };
                
                _tiles[x][y] = sprite;
            }
        }
        
        _snakeGame = new SnakeGame(Rows, Columns);
        
        for (var x = 0; x < _snakeGame.Tiles.Length; x++)
        {
            for (var y = 0; y < _snakeGame.Tiles[x].Length; y++)
            {
                _tiles[x][y].Texture = _snakeGame.Tiles[x][y].TileType switch
                {
                    Tile.Type.WallBottom => _wallTexture,
                    Tile.Type.WallRight => _wallTexture,
                    Tile.Type.WallLeft => _wallTexture,
                    Tile.Type.WallTop => _wallTexture,
                    Tile.Type.Empty => _backgroundTexture,
                    _ => _backgroundTexture
                };
            }
        }
        
        _snakeGame.StartGame();
        
        window.Display();
        
        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(Color.Black);
            foreach ((Tile tile, Position position) in _snakeGame.Update())
            {
                _tiles[position.Row][position.Column].Texture = tile.TileType switch
                {
                    Tile.Type.WallTop => _wallTexture,
                    Tile.Type.WallBottom => _wallTexture,
                    Tile.Type.WallLeft => _wallTexture,
                    Tile.Type.WallRight => _wallTexture,
                    Tile.Type.Empty => _backgroundTexture,
                    Tile.Type.Fruit => _fruitTexture,
                    Tile.Type.TailLeftRight => _snakeTailLeftRightTexture,
                    Tile.Type.TailLeftLeft => _snakeTailLeftRightTexture,
                    Tile.Type.TailUpDown => _snakeTailUpDownTexture,
                    Tile.Type.TailUpUp => _snakeTailUpDownTexture,
                    Tile.Type.TailUpRight => _snakeTailUpRightTexture,
                    Tile.Type.TailRightUp => _snakeTailUpRightTexture,
                    Tile.Type.TailUpLeft => _snakeTailUpLeftTexture,
                    Tile.Type.TailLeftUp => _snakeTailUpLeftTexture,
                    Tile.Type.TailLeftDown => _snakeTailDownLeftTexture,
                    Tile.Type.TailDownLeft => _snakeTailDownLeftTexture,
                    Tile.Type.TailRightDown => _snakeTailDownRightTexture,
                    Tile.Type.TailDownRight => _snakeTailDownRightTexture,
                    Tile.Type.TailEndRight => _snakeTailEndRightTexture,
                    Tile.Type.TailEndLeft => _snakeTailEndLeftTexture,
                    Tile.Type.TailEndUp => _snakeTailEndUpTexture,
                    Tile.Type.TailEndDown => _snakeTailEndDownTexture,
                    Tile.Type.HeadLeft => _snakeHeadLeftTexture,
                    Tile.Type.HeadUp => _snakeHeadUpTexture,
                    Tile.Type.HeadDown => _snakeHeadDownTexture,
                    Tile.Type.HeadRight => _snakeHeadRightTexture,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            foreach (Sprite[] columns in _tiles)
            {
                foreach (Sprite sprite in columns)
                {
                    window.Draw(sprite);
                }
            }
            
            window.Display();
            
            Thread.Sleep(200);
        }
    }
    
    

    private static void WindowOnKeyPressed(object? sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Left:
                _snakeGame.LeftPress();
                break;
            case Keyboard.Key.Right:
                _snakeGame.RightPress();
                break;
            case Keyboard.Key.Up:
                _snakeGame.UpPress();
                break;
            case Keyboard.Key.Down:
                _snakeGame.DownPress();
                break;
        }
    }
}