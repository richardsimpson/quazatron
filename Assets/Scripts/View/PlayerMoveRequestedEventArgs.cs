using System;

public enum Direction {
    UP,
    DOWN
}

public class PlayerMoveRequestedEventArgs : EventArgs
{
    public Direction direction;

    public PlayerMoveRequestedEventArgs(Direction direction)
    {
        this.direction = direction;
    }    

}

