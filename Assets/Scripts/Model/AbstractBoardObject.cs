using System;
using System.Collections.Generic;

public class AbstractBoardObject : BoardObject
{
    private List<BoardObject> outputs = new List<BoardObject>();

	public AbstractBoardObject() {
		
	}

	protected void AddOutput(BoardObject output) {
        this.outputs.Add(output);
	}

    public List<BoardObject> getOutputs() {
		return this.outputs;
	}

    public virtual void inputActivated(BoardObject input) {
        throw new NotImplementedException();
    }
}

