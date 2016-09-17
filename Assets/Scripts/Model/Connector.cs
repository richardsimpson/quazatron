using System;
using System.Collections.Generic;

public class Connector : AbstractBoardObject
{
    public Connector(List<AbstractBoardObject> outputs) : base() {
        for (int i = 0 ; i < outputs.Count ; i++) {
            AddOutput(outputs[i]);
            outputs[i].AddInput(this);
        }
	}

    public Connector(AbstractBoardObject output) : base() {
        AddOutput(output);
        output.AddInput(this);
    }
}

