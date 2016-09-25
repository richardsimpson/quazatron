using System;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorController : AbstractWireController {
    
    private SpriteRenderer connectorRenderer = null;

    // Use this for initialization
    public override void Start () {
        base.Start();

        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Connector") {
                connectorRenderer = comps[i];
            }
        }

        if (connectorRenderer == null) {
            throw new Exception("Cannot locate connector component");
        }

        if (owner == PlayerNumber.PLAYER1) {
            connectorRenderer.color = YELLOW;
        }
        else {
            connectorRenderer.color = BLUE;
        }
    }

}
