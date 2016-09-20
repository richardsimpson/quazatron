using System;
using UnityEngine;

public class Target : AbstractBoardObject
{
    public Target() : base() {
    }

    public override void inputActivated(BoardObject input, PlayerNumber playerNumber) {
        setActivated(true);
        OnBoardObjectActivated(new BoardObjectActivatedEventArgs(playerNumber));
    }


}

