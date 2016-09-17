using System;

public class Initiator : AbstractBoardObject
{
    public Initiator(AbstractBoardObject output) : base() {
        AddOutput(output);
        output.AddInput(this);
    }
}

