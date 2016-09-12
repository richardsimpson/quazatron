using UnityEngine;
using System.Collections;

public class WireController : MonoBehaviour {

    private Color YELLOW = new Color(1F, 1F, 0F);

    private SpriteRenderer wireRenderer;

    // Use this for initialization
	void Start () {
        wireRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onWireActivated()
    {
        wireRenderer.color = YELLOW;
    }
}
