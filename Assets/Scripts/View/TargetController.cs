using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

    private Color YELLOW = new Color(1F, 1F, 0F);

    private SpriteRenderer targetRenderer;

	// Use this for initialization
	void Start () {
        targetRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onTargetActivated()
    {
        targetRenderer.color = YELLOW;
    }
}
