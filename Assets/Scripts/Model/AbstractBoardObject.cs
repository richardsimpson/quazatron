using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBoardObject : BoardObject
{
    public event BoardObjectActivatedEventHandler boardObjectActivated;
    public event BoardObjectDeactivatedEventHandler boardObjectDeactivated;

    private List<BoardObject> outputs = new List<BoardObject>();
    private List<AbstractBoardObject> inputs = new List<AbstractBoardObject>();

    private bool activated = false;

	protected void AddOutput(BoardObject output) {
        this.outputs.Add(output);
	}

    public List<BoardObject> getOutputs() {
		return this.outputs;
	}

    public void AddInput(AbstractBoardObject input)
    {
        this.inputs.Add(input);
    }

    protected void OnBoardObjectActivated(EventArgs eventArgs) {
        Debug.Log("OnBoardObjectActivated.");
        if (boardObjectActivated != null)
            boardObjectActivated(this, eventArgs);
    }

    protected void OnBoardObjectDeactivated(EventArgs eventArgs) {
        Debug.Log("OnBoardObjectDeactivated.");
        if (boardObjectDeactivated != null)
            boardObjectDeactivated(this, eventArgs);
    }

    public void inputActivated(BoardObject input) {
        // Only execute OnBoardObjectActivated and the outputs' inputActivated if ALL inputs are activated.

        bool allInputsActivated = true;
        for (int i = 0 ; i < inputs.Count ; i++) {
            if (!inputs[i].activated) {
                allInputsActivated = false;
            }
        }

        if (allInputsActivated) {
            this.activated = true;
            OnBoardObjectActivated(EventArgs.Empty);

            for (int i = 0 ; i < this.outputs.Count ; i++) {
                outputs[i].inputActivated(this);
            }
        }
    }

    public void inputDeactivated(BoardObject input) {
        this.activated = false;
        OnBoardObjectDeactivated(EventArgs.Empty);

        for (int i = 0 ; i < this.outputs.Count ; i++) {
            outputs[i].inputDeactivated(this);
        }
    }

    public bool isActivated() {
        return this.activated;
    }

}

