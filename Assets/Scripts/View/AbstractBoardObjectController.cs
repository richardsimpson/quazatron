using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBoardObjectController : MonoBehaviour
{
    // This is the player that 'owns' the component.  This does NOT change.
    protected PlayerNumber owner;

    private bool activated = false;

    public AbstractBoardObjectController() {

    }

    public void setOwner(PlayerNumber owner) {
        this.owner = owner;
    }

    public virtual void onActivated(PlayerNumber playerNumber, Side player1side) {
        this.activated = true;
    }

    public virtual void onDeactivated() {
        this.activated = false;
    }

    public bool isActivated() {
        return this.activated;
    }

}

