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

    public override void onActivated()
    {
        wireRenderer.color = YELLOW;
    }
}
