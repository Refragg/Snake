// FIXME: This file is unused but has some good concepts that could be used
namespace Snake;

public class Snake
{
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down
    }

    public enum TailType
    {
        Start,
        Normal,
        End
    }

    public enum TailDirection
    {
        UpDown,
        UpRight,
        UpLeft,
        LeftDown,
        LeftRight,
        RightDown
    }

    public Position HeadPosition;
    public List<(Position, TailDirection)> Tail = new List<(Position, TailDirection)>();
    
    public Snake(Position startPos)
    {
        HeadPosition = startPos;
        startPos.Column++;
        Tail.Add((startPos, TailDirection.LeftRight));
    }
    
    private Direction _OldDirection;
    
    public (Position headNewPos, Position tailEndNewPos) Move(Direction direction)
    {
        Position positionDelta = direction switch
        {
            Direction.Left => (0, -1),
            Direction.Right => (0, 1),
            Direction.Up => (-1, 0),
            Direction.Down => (1, 0)
        };

        HeadPosition += positionDelta;
        (Position, TailDirection) tailEnd = Tail[^1];
        tailEnd.Item1 += positionDelta;
        
        _OldDirection = direction;

        return (HeadPosition, tailEnd.Item1);
    }
}