namespace Snake;

public class Tile
{
    // FIXME: Reduce the number of tail types somehow (there's some duplicates)
    public enum Type
    {
        WallTop,
        WallBottom,
        WallLeft,
        WallRight,
        Empty,
        Fruit,
        TailUpUp,
        TailUpDown,
        TailLeftLeft,
        TailLeftRight,
        TailUpRight,
        TailRightUp,
        TailUpLeft,
        TailLeftUp,
        TailLeftDown,
        TailDownLeft,
        TailRightDown,
        TailDownRight,
        TailEndLeft,
        TailEndRight,
        TailEndUp,
        TailEndDown,
        HeadUp,
        HeadDown,
        HeadLeft,
        HeadRight
    }

    public Type TileType { get; set; }

    public Tile(Type type)
    {
        TileType = type;
    }

    public Tile()
    {
        TileType = Type.Empty;
    }
}