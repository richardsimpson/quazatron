using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void WireActivatedEventHandler(Wire sender, EventArgs e);

public class Wire : AbstractBoardObject
{
    public event WireActivatedEventHandler wireActivated;

    public Wire(BoardObject output) : base() {
        AddOutput(output);
	}

    private void OnWireActivated(EventArgs eventArgs) {
        Debug.Log("OnWireActivated.");
        if (wireActivated != null)
            wireActivated(this, eventArgs);
    }

    public override void inputActivated(BoardObject input) {
        OnWireActivated(EventArgs.Empty);

        List<BoardObject> outputs = getOutputs();
        for (int i = 0 ; i < outputs.Count ; i++) {
            outputs[i].inputActivated(this);
        }

    }

}


