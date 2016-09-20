using System;
using UnityEngine;

public class InitiatorController : AbstractBoardObjectController {

    private SpriteRenderer wireRenderer = null;
    private SpriteRenderer initiatorRenderer = null;

    // Use this for initialization
    void Start () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Wire") {
                wireRenderer = comps[i];
            }
            else if (comps[i].sprite.name == "Zap") {
                initiatorRenderer = comps[i];
            }
        }

        if (wireRenderer == null) {
            throw new Exception("Cannot locate wire component");
        }

        if (initiatorRenderer == null) {
            throw new Exception("Cannot locate initiator component");
        }

        if (playerNumber == PlayerNumber.PLAYER1) {
            initiatorRenderer.color = YELLOW;
        }
        else {
            initiatorRenderer.color = BLUE;
        }
    }

    public override void onActivated(PlayerNumber playerNumber)
    {
        base.onActivated(playerNumber);
        wireRenderer.color = getColourForPlayerNumber(playerNumber);
    }

    public override void onDeactivated()
    {
        wireRenderer.color = BLACK;
    }

}
