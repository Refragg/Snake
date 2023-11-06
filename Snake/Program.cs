namespace Snake;

// FIXME: Try to abstract some stuff away?
internal class Program
{
    private static SnakeGame _snakeGame;

    private static Task KeyboardPollTask = new (KeyboardPollLoop);

    public static void Main(string[] args)
    {
        //ConsoleSnake(args);
        GraphicalSnake.Snake(args);

    }
    
    public static void ConsoleSnake(string[] args)
    {
        _snakeGame = new SnakeGame(15, 25);
        PrintGame();
        Console.WriteLine("Press ZQSD to move or enter to just update game");
        _snakeGame.StartGame();
        Console.ReadLine();
        KeyboardPollTask.Start();
        while (_snakeGame.CurrentState != SnakeGame.State.Dead)
        {
            Console.Clear();
            _snakeGame.Update();
            PrintGame();
            Thread.Sleep(200);
        }

        Console.WriteLine("You are dead");
        KeyboardPollTask.Wait();
    }

    public static void KeyboardPollLoop()
    {
        while (_snakeGame.CurrentState != SnakeGame.State.Dead)
        {
            
            char c = Console.ReadKey(true).KeyChar;
            switch (char.ToUpper(c))
            {
                case 'Z':
                    _snakeGame.UpPress();
                    break;
                case 'Q':
                    _snakeGame.LeftPress();
                    break;
                case 'S':
                    _snakeGame.DownPress();
                    break;
                case 'D':
                    _snakeGame.RightPress();
                    break;
                case 'P':
                    Environment.Exit(0);
                    break;
            }
        }
    }

    // FIXME: Print all the tile types properly
    public static void PrintGame()
    {
        foreach (Tile[] columns in _snakeGame.Tiles)
        {
            foreach (Tile tile in columns)
            {
                char toDraw = tile.TileType switch
                {
                    Tile.Type.WallTop => '-',
                    Tile.Type.WallBottom => '^',
                    Tile.Type.WallLeft => '>',
                    Tile.Type.WallRight => '<',
                    Tile.Type.Empty => ' ',
                    Tile.Type.Fruit => 'F',
                    Tile.Type.TailLeftRight => 'T',
                    Tile.Type.TailEndRight => 'E',
                    Tile.Type.HeadLeft => 'H',
                    _ => 'I'
                };
                
                Console.Write(toDraw);
            }

            Console.Write(Environment.NewLine);
        }
    }
}