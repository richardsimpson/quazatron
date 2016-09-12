using System;

public class PlayerMovedEventArgs : EventArgs
{
    public int position;

    public PlayerMovedEventArgs(int position)
    {
        this.position = position;
    }    

}

