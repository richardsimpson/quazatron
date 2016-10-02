using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractWireController : AbstractBoardObjectController {

    public Material yellowLeftWire;
    public Material yellowRightWire;
    public Material blueLeftWire;
    public Material blueRightWire;
    public Material blackWire;

    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private PlayerNumber activePlayerNumber;
    private float currentOffset = 0.0f;

    private float period = 0.0125f;

    private float nextActionTime = 0.0f;

    // Use this for initialization
    public virtual void Start () {
        MeshRenderer[] comps = GetComponentsInChildren<MeshRenderer>();
    
        for (int i = 0 ; i < comps.Length ; i++) {
            this.meshRenderers.Add(comps[i]);
        }
    }

    void FixedUpdate () {
        if (isActivated()) {
            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + period;

                currentOffset = currentOffset + 0.025f;
                if (currentOffset >= 1.0f) {
                    currentOffset = 0.0f;
                }

                for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
                    this.meshRenderers[i].material.SetTextureOffset("_MainTex", new Vector2(currentOffset, 0.0f));
                }
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

        if (this.owner != PlayerNumber.PLAYER1 && this.owner != PlayerNumber.PLAYER2) {
            throw new Exception("Unexpected player number " + this.owner + " for owner");
        }

        if (this.activePlayerNumber != PlayerNumber.PLAYER1 && this.activePlayerNumber != PlayerNumber.PLAYER2) {
            throw new Exception("Unexpected player number " + this.activePlayerNumber + " for activePlayerNumber");
        }

        Material activeMaterial;
        if (this.owner == PlayerNumber.PLAYER1) {
            if (this.activePlayerNumber == PlayerNumber.PLAYER1) {
                activeMaterial = yellowLeftWire;
            }
            else {
                activeMaterial = blueLeftWire;
            }
        }
        else {
            if (this.activePlayerNumber == PlayerNumber.PLAYER1) {
                activeMaterial = yellowRightWire;
            }
            else {
                activeMaterial = blueRightWire;
            }
        }

        for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
            this.meshRenderers[i].material = activeMaterial; //getColourForPlayerNumber(playerNumber);
        }
    }

    public override void onDeactivated()
    {
        base.onDeactivated();

        for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
            this.meshRenderers[i].material = blackWire;
        }
        //meshRenderer.color = BLACK;
    }
}
