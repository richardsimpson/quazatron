using System;

public enum Direction {
    UP,
    DOWN
}

public class PlayerMoveRequestedEventArgs : EventArgs
{
    public PlayerNumber playerNumber;
    public Direction direction;

    public PlayerMoveRequestedEventArgs(PlayerNumber playerNumber, Direction direction)
    {
        this.playerNumber = playerNumber;
        this.direction = direction;
    }    

}

