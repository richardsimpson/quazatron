using System;
using UnityEngine;

public class TerminatorController : AbstractWireController {

    private SpriteRenderer terminatorRenderer = null;

    // Use this for initialization
    public override void Start () {
        base.Start();

        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Zap") {
                terminatorRenderer = comps[i];
            }
        }

        if (terminatorRenderer == null) {
            throw new Exception("Cannot locate terminator component");
        }

        if (owner == PlayerNumber.PLAYER1) {
            terminatorRenderer.color = YELLOW;
        }
        else {
            terminatorRenderer.color = BLUE;
        }
    }

}
