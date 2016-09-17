using System;
using System.Collections.Generic;
using UnityEngine;

public class Wire : AbstractBoardObject
{
    public Wire(AbstractBoardObject output) : base() {
        AddOutput(output);
        output.AddInput(this);
	}

}


