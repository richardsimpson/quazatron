using UnityEngine;
using System.Collections.Generic;

public class ConnectorController : AbstractBoardObjectController {
    private List<SpriteRenderer> wireRenderers = new List<SpriteRenderer>();

    // Use this for initialization
    void Start () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Wire") {
                wireRenderers.Add(comps[i]);
            }
        }
    }

    public override void onActivated()
    {
        for (int i = 0 ; i < wireRenderers.Count ; i++) {
            wireRenderers[i].color = YELLOW;
        }
    }

}
