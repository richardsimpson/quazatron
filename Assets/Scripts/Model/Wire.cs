using System;
using System.Collections.Generic;
using UnityEngine;

public class Wire : AbstractBoardObject
{
    public Wire(BoardObject output) : base() {
        AddOutput(output);
	}

}


