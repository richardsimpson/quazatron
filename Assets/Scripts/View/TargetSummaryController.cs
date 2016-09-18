using UnityEngine;
using System.Collections;

public class TargetSummaryController : MonoBehaviour {

    private Color YELLOW = new Color(1F, 1F, 0F);

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {

    }

    public void onActivated()
    {
        spriteRenderer.color = YELLOW;
    }
}
