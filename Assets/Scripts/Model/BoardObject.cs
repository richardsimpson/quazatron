using System;
using System.Collections.Generic;

public delegate void BoardObjectActivatedEventHandler(BoardObject sender, BoardObjectActivatedEventArgs e);
public delegate void BoardObjectDeactivatedEventHandler(BoardObject sender, EventArgs e);

public class BoardObjectActivatedEventArgs : EventArgs
{
    public PlayerNumber playerNumber;

    public BoardObjectActivatedEventArgs(PlayerNumber playerNumber)
    {
        this.playerNumber = playerNumber;
    }    

}

public interface BoardObject
{
    event BoardObjectActivatedEventHandler boardObjectActivated;
    event BoardObjectDeactivatedEventHandler boardObjectDeactivated;

    List<BoardObject> getOutputs();

    void inputActivated(BoardObject input, PlayerNumber playerNumber);

    // model is only passed in so that the Initiator can use it to call StartCoroutine
    void inputDeactivated(Model model, BoardObject input);
}
