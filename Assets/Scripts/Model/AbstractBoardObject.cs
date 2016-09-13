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

    public virtual void inputActivated(BoardObject input) {
        throw new NotImplementedException();
    }
}

