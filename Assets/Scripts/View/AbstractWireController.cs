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

    public override void onActivated(PlayerNumber playerNumber, Side player1side)
    {
        base.onActivated(playerNumber, player1side);

        if (playerNumber != PlayerNumber.PLAYER1 && playerNumber != PlayerNumber.PLAYER2) {
            throw new Exception("Unexpected player number " + playerNumber + " for activePlayerNumber");
        }

        // The colour of the wire is not dependant on the player and the side of the zap / component.
        // It's the side that player1 is using.  that way, if player 1 is using the right side, and the enemy (on the left) fires into
        // a wire, the player number will be 2, and the side is RIGHT (yellow wire).
        Material activeMaterial;
        if (Side.LEFT == player1side) {
            if (playerNumber == PlayerNumber.PLAYER1) {
                activeMaterial = yellowLeftWire;
            }
            else {
                activeMaterial = blueLeftWire;
            }
        }
        else {
            if (playerNumber == PlayerNumber.PLAYER1) {
                activeMaterial = blueRightWire;
            }
            else {
                activeMaterial = yellowRightWire;
            }
        }

        for (int i = 0 ; i < this.meshRenderers.Count ; i++) {
            this.meshRenderers[i].material = activeMaterial;
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
