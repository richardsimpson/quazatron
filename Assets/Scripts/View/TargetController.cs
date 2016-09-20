using UnityEngine;
using System.Collections;

public class TargetController : AbstractBoardObjectController {

    private SpriteRenderer targetRenderer;

	// Use this for initialization
	void Start () {
        targetRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void onActivated(PlayerNumber playerNumber)
    {
        base.onActivated(playerNumber);
        targetRenderer.color = getColourForPlayerNumber(playerNumber);
    }

    public override void onDeactivated()
    {
        // do nothing here for now.  we do not want to swap colour unless the other player already has a 
        // zap going to this target, or if they create a new one.
    }
}
