using System;
using System.Collections.Generic;

public class Connector : AbstractBoardObject
{
    public Connector(List<BoardObject> outputs) : base() {
        for (int i = 0 ; i < outputs.Count ; i++) {
            AddOutput(outputs[i]);
        }
	}

    public Connector(BoardObject output) : base() {
        AddOutput(output);
    }
}

