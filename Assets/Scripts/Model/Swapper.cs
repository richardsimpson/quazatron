using System;

public class Swapper : AbstractBoardObject
{
    public Swapper(AbstractBoardObject output) : base() {
        AddOutput(output);
        output.AddInput(this);
    }
}

