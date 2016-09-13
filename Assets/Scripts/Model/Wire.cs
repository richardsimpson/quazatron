using System;
using System.Collections.Generic;
using UnityEngine;

public class Wire : AbstractBoardObject
{
    public Wire(BoardObject output) : base() {
        AddOutput(output);
	}

    public override void inputActivated(BoardObject input) {
        OnBoardObjectActivated(EventArgs.Empty);

        List<BoardObject> outputs = getOutputs();
        for (int i = 0 ; i < outputs.Count ; i++) {
            outputs[i].inputActivated(this);
        }

    }

}


