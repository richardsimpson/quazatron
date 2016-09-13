using System;
using System.Collections.Generic;

public delegate void BoardObjectActivatedEventHandler(BoardObject sender, EventArgs e);

public interface BoardObject
{
    event BoardObjectActivatedEventHandler boardObjectActivated;

    List<BoardObject> getOutputs();

    void inputActivated(BoardObject input);
}
