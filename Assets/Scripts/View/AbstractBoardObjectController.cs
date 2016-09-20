using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBoardObjectController : MonoBehaviour
{
    protected Color YELLOW = new Color(1F, 1F, 0F);
    protected Color BLUE = new Color(0F, 0F, 1F);
    protected Color BLACK = new Color(0F, 0F, 0F);

    protected PlayerNumber playerNumber;

    private bool activated = false;

    public AbstractBoardObjectController() {

    }

    public void setPlayerNumber(PlayerNumber playerNumber) {
        this.playerNumber = playerNumber;
    }

    public virtual void onActivated() {
        this.activated = true;
    }

    public virtual void onDeactivated() {
        this.activated = false;
    }

    public bool isActivated() {
        return this.activated;
    }

}

