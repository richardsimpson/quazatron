using System;
using System.Collections.Generic;

public delegate void BoardObjectActivatedEventHandler(BoardObject sender, EventArgs e);
public delegate void BoardObjectDeactivatedEventHandler(BoardObject sender, EventArgs e);

public interface BoardObject
{
    event BoardObjectActivatedEventHandler boardObjectActivated;
    event BoardObjectDeactivatedEventHandler boardObjectDeactivated;

    List<BoardObject> getOutputs();

    void inputActivated(BoardObject input);
    void inputDeactivated(BoardObject input);
}
