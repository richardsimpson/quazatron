using UnityEngine;

public class WireController : AbstractBoardObjectController {

    private SpriteRenderer wireRenderer;

    // Use this for initialization
	void Start () {
        wireRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void onActivated(PlayerNumber playerNumber)
    {
        base.onActivated(playerNumber);
        wireRenderer.color = getColourForPlayerNumber(playerNumber);
    }

    public override void onDeactivated()
    {
        wireRenderer.color = BLACK;
    }
}
