using System;
using UnityEngine;

public class TerminatorController : AbstractBoardObjectController {

    private SpriteRenderer wireRenderer = null;

    // Use this for initialization
    void Start () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Wire") {
                wireRenderer = comps[i];
            }
        }

        if (wireRenderer == null) {
            throw new Exception("Cannot locate wire component");
        }
    }

    public override void onActivated()
    {
        wireRenderer.color = YELLOW;
    }

    public override void onDeactivated()
    {
        wireRenderer.color = BLACK;
    }

}
