using System;
using UnityEngine;

public delegate void TargetActivatedEventHandler(Target sender, EventArgs e);

public class Target : AbstractBoardObject
{
    public event TargetActivatedEventHandler targetActivated;

    public Target() : base() {
    }

    private void OnTargetActivated(EventArgs eventArgs) {
        Debug.Log("OnTargetActivated.");
        if (targetActivated != null)
            targetActivated(this, eventArgs);
    }

    public override void inputActivated(BoardObject input) {
        OnTargetActivated(EventArgs.Empty);
    }
}

