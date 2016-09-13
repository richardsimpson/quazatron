using System;
using UnityEngine;

public class Target : AbstractBoardObject
{
    public Target() : base() {
    }

    public override void inputActivated(BoardObject input) {
        OnBoardObjectActivated(EventArgs.Empty);
    }
}

