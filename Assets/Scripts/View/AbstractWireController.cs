using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractWireController : AbstractBoardObjectController {

    public Material yellowWire;
    public Material blackWire;

    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private PlayerNumber activePlayerNumber;
    private float currentOffset = 0.0f;

    // Use this for initialization
    public virtual void Start () {
        MeshRenderer[] comps = GetComponentsInChildren<MeshRenderer>();
    
        for (int i = 0 ; i < comps.Length ; i++) {
            this.meshRenderers.Add(comps[i]);
        }
    }

    // Update is called once per frame
    void Update () {
        if (isActivated() && this.activePlayerNumber == PlayerNumber.PLAYER1) {
            currentOffset = currentOffset + 0.05f;
            if (currentOffset >= 1.0f) {
                currentOffset = 0.0f;
            }

            for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
                this.meshRenderers[i].material.SetTextureOffset("_MainTex", new Vector2(currentOffset, 0.0f));
            }
        }
    }

    public override void onActivated(PlayerNumber playerNumber)
    {
        // playerNumber indicates the colour (PLAYER1 = YELLOW, PLAYER2 = BLUE)
        // the inherited 'owner' indicates the side of the board (PLAYER1 = left, PLAYER2 = right)
        //
        // with this information, we can determine which material to use, to display the 'flow' on the wire.

        base.onActivated(playerNumber);

        this.activePlayerNumber = playerNumber;

        for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
            this.meshRenderers[i].material = yellowWire; //getColourForPlayerNumber(playerNumber);
        }
    }

    public override void onDeactivated()
    {
        for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
            this.meshRenderers[i].material = blackWire;
        }
        //meshRenderer.color = BLACK;
    }
}
