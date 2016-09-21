using System;
using System.Collections;
using UnityEngine;

public class TargetController : AbstractBoardObjectController {

    private float period = 0.03f;

    private float nextActionTime = 0.0f;

    private SpriteRenderer targetRenderer;

    private bool controlDisputed = false;

	// Use this for initialization
	void Start () {
        targetRenderer = GetComponent<SpriteRenderer>();
        nextActionTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (controlDisputed) {
            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + period;

                // TODO: Alternate between yellow, blue and black.
                if (targetRenderer.color == YELLOW) {
                    targetRenderer.color = BLUE;
                }
                else if (targetRenderer.color == BLUE) {
                    targetRenderer.color = BLACK;
                }
                else if (targetRenderer.color == BLACK) {
                    targetRenderer.color = YELLOW;
                }
            }

        }
	
	}

    public override void onActivated(PlayerNumber playerNumber)
    {
        // TODO: Account for player numbers PLAYER1, PLAYER2 and BOTH.
        if ((PlayerNumber.PLAYER1 == playerNumber) || (PlayerNumber.PLAYER2 == playerNumber)) {
            this.controlDisputed = false;
            targetRenderer.color = getColourForPlayerNumber(playerNumber);
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
