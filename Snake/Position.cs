namespace Snake;

public struct Position
{
    public int Row;

    public int Column;

    public Position()
    {
        
    }

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }
    
    public static Position operator +(Position first, Position second)
    {
        first.Row += second.Row;
        first.Column += second.Column;

        return first;
    }
    
    public static Position operator -(Position first, Position second)
    {
        first.Row -= second.Row;
        first.Column -= second.Column;

        return first;
    }
    
    public static bool operator ==(Position first, Position second)
    {
        return first.Row == second.Row && first.Column == second.Column;
    }

    public static bool operator !=(Position first, Position second)
    {
        return !(first == second);
    }

    public static implicit operator Position((int row, int column) position)
    {
        return new Position(position.row, position.column);
    }
}