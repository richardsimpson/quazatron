using System;

public class Swapper : AbstractBoardObject
{
    public Swapper(BoardObject output) : base() {
        AddOutput(output);
    }
}

