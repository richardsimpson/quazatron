using System;
using UnityEngine;

public class TerminatorController : AbstractBoardObjectController {

    private SpriteRenderer wireRenderer = null;
    private SpriteRenderer terminatorRenderer = null;

    // Use this for initialization
    void Start () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Wire") {
                wireRenderer = comps[i];
            }
            else if (comps[i].sprite.name == "Zap") {
                terminatorRenderer = comps[i];
            }
        }

        if (wireRenderer == null) {
            throw new Exception("Cannot locate wire component");
        }

        if (terminatorRenderer == null) {
            throw new Exception("Cannot locate terminator component");
        }

        if (playerNumber == PlayerNumber.PLAYER1) {
            terminatorRenderer.color = YELLOW;
        }
        else {
            terminatorRenderer.color = BLUE;
        }
    }

    public override void onActivated()
    {
        base.onActivated();
        wireRenderer.color = YELLOW;
    }

    public override void onDeactivated()
    {
        wireRenderer.color = BLACK;
    }

}
