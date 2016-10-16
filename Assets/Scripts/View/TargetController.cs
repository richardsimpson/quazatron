using System;
using System.Collections;
using UnityEngine;

public class TargetController : AbstractBoardObjectController {

    private float period = 0.03f;

    private float nextActionTime = 0.0f;

    private SpriteRenderer targetRenderer;

    private bool controlDisputed = false;

	// Use this for initialization.  Using Awake instead of Start, because the controller will cause a call
    // to onActivated before Start() is called.
	void Awake () {

        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Target") {
                targetRenderer = comps[i];
            }
        }

        if (targetRenderer == null) {
            throw new Exception("Cannot locate correct SpriteRenderer component");
        }

        nextActionTime = Time.time;
	}
	
	void FixedUpdate () {
        if (controlDisputed) {
            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + period;

                // Alternate between yellow, blue and black.
                if (targetRenderer.color == ViewConstants.YELLOW) {
                    targetRenderer.color = ViewConstants.BLUE;
                }
                else if (targetRenderer.color == ViewConstants.BLUE) {
                    targetRenderer.color = ViewConstants.BLACK;
                }
                else if (targetRenderer.color == ViewConstants.BLACK) {
                    targetRenderer.color = ViewConstants.YELLOW;
                }
            }

        }
	
	}

    public override void onActivated(PlayerNumber playerNumber, Side player1side)
    {
        if ((PlayerNumber.PLAYER1 == playerNumber) || (PlayerNumber.PLAYER2 == playerNumber)) {
            this.controlDisputed = false;
            targetRenderer.color = ViewConstants.getColourForPlayerNumberAndSide(playerNumber, player1side);
        }
        else if (PlayerNumber.BOTH == playerNumber) {
            this.controlDisputed = true;
        }
        else {
            this.controlDisputed = false;
        }
    }

    public override void onDeactivated()
    {
        throw new Exception("TargetController.onDeactivated should not get called");
    }
}
