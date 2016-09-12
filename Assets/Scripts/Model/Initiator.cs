using System;

public class Initiator : AbstractBoardObject
{
    public Initiator(BoardObject output) : base() {
        AddOutput(output);
    }
}

