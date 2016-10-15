using System;
using UnityEngine;

public class SwapperController : AbstractWireController {
    
    private SpriteRenderer swapperRenderer = null;

    // Use this for initialization
    public override void Start () {
        base.Start();

        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Swapper") {
                swapperRenderer = comps[i];
            }
        }

        if (swapperRenderer == null) {
            throw new Exception("Cannot locate swapper component");
        }

        if (owner == PlayerNumber.PLAYER1) {
            swapperRenderer.color = ViewConstants.BLUE;
        }
        else {
            swapperRenderer.color = ViewConstants.YELLOW;
        }
    }

    private PlayerNumber getOppositePlayerForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return PlayerNumber.PLAYER2;
        }

        return PlayerNumber.PLAYER1;
    }

    public override void onActivated(PlayerNumber playerNumber, Side player1side)
    {
        base.onActivated(getOppositePlayerForPlayerNumber(playerNumber), player1side);
    }

}
