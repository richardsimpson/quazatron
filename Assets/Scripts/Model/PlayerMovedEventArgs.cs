using System;

public class PlayerMovedEventArgs : EventArgs
{
    public int position;
    public PlayerNumber playerNumber;

    public PlayerMovedEventArgs(PlayerNumber playerNumber, int position)
    {
        this.position = position;
        this.playerNumber = playerNumber;
    }    

}

