﻿using System;
using UnityEngine;

public class SwapperController : AbstractBoardObjectController {
    
    private SpriteRenderer wireRenderer = null;
    private SpriteRenderer swapperRenderer = null;

    // Use this for initialization
    void Start () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Wire") {
                wireRenderer = comps[i];
            }
            else if (comps[i].sprite.name == "Swapper") {
                swapperRenderer = comps[i];
            }
        }

        if (wireRenderer == null) {
            throw new Exception("Cannot locate wire component");
        }

        if (swapperRenderer == null) {
            throw new Exception("Cannot locate swapper component");
        }

        if (playerNumber == PlayerNumber.PLAYER1) {
            swapperRenderer.color = BLUE;
        }
        else {
            swapperRenderer.color = YELLOW;
        }
    }

    protected Color getOppositeColourForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return BLUE;
        }

        return YELLOW;
    }

    public override void onActivated(PlayerNumber playerNumber)
    {
        base.onActivated(playerNumber);
        wireRenderer.color = getOppositeColourForPlayerNumber(playerNumber);
    }

    public override void onDeactivated()
    {
        wireRenderer.color = BLACK;
    }
}
