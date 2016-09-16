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

    public override void onActivated()
    {
        targetRenderer.color = YELLOW;
    }
}
