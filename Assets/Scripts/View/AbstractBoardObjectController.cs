using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBoardObjectController : MonoBehaviour
{
    protected Color YELLOW = new Color(1F, 1F, 0F);
    protected Color BLUE = new Color(0F, 0F, 1F);
    protected Color BLACK = new Color(0F, 0F, 0F);

    // This is the player that 'owns' the component.  This does NOT change.
    protected PlayerNumber owner;

    private bool activated = false;

    public AbstractBoardObjectController() {

    }

    public void setOwner(PlayerNumber owner) {
        this.owner = owner;
    }

    public virtual void onActivated(PlayerNumber playerNumber) {
        this.activated = true;
    }

    public virtual void onDeactivated() {
        this.activated = false;
    }

    public bool isActivated() {
        return this.activated;
    }

    protected Color getColourForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return YELLOW;
        }

        return BLUE;
    }
}

