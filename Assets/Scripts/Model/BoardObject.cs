using System;
using System.Collections.Generic;

public interface BoardObject
{
    List<BoardObject> getOutputs();

    void inputActivated(BoardObject input);
}
