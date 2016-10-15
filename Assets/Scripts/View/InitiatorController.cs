using System;
using UnityEngine;

public class InitiatorController : AbstractWireController {

    private SpriteRenderer initiatorRenderer = null;

    // Use this for initialization
    public override void Start() {
        base.Start();

        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Zap") {
                initiatorRenderer = comps[i];
            }
        }

        if (initiatorRenderer == null) {
            throw new Exception("Cannot locate initiator component");
        }

        if (owner == PlayerNumber.PLAYER1) {
            initiatorRenderer.color = ViewConstants.YELLOW;
        }
        else {
            initiatorRenderer.color = ViewConstants.BLUE;
        }
    }

}
