using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBoardObject : BoardObject
{
    public event BoardObjectActivatedEventHandler boardObjectActivated;

    private List<BoardObject> outputs = new List<BoardObject>();

	protected void AddOutput(BoardObject output) {
        this.outputs.Add(output);
	}

    public List<BoardObject> getOutputs() {
		return this.outputs;
	}

    protected void OnBoardObjectActivated(EventArgs eventArgs) {
        Debug.Log("OnBoardObjectActivated.");
        if (boardObjectActivated != null)
            boardObjectActivated(this, eventArgs);
    }

    // TODO: For Connectors, only execute OnBoardObjectActivated and the outputs' inputActivated if ALL inputs are activated.
    //       This means the board objects ALSO need to know what their inputs are.
    public void inputActivated(BoardObject input) {
        OnBoardObjectActivated(EventArgs.Empty);

        for (int i = 0 ; i < this.outputs.Count ; i++) {
            outputs[i].inputActivated(this);
        }

    }

}

