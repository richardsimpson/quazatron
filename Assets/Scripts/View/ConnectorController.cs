using System;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorController : AbstractBoardObjectController {
    
    private List<SpriteRenderer> wireRenderers = new List<SpriteRenderer>();
    private SpriteRenderer connectorRenderer = null;

    // Use this for initialization
    void Start () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Wire") {
                wireRenderers.Add(comps[i]);
            }
            else if (comps[i].sprite.name == "Connector") {
                connectorRenderer = comps[i];
            }
        }

        if (connectorRenderer == null) {
            throw new Exception("Cannot locate connector component");
        }

        if (playerNumber == PlayerNumber.PLAYER1) {
            connectorRenderer.color = YELLOW;
        }
        else {
            connectorRenderer.color = BLUE;
        }
        }

    public override void onActivated()
    {
        for (int i = 0 ; i < wireRenderers.Count ; i++) {
            wireRenderers[i].color = YELLOW;
        }
    }

    public override void onDeactivated()
    {
        for (int i = 0 ; i < wireRenderers.Count ; i++) {
            wireRenderers[i].color = BLACK;
        }
    }

}
