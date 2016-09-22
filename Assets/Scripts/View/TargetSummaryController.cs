using UnityEngine;
using System.Collections;

public class TargetSummaryController : MonoBehaviour {

    private Color YELLOW = new Color(1F, 1F, 0F);
    private Color BLUE = new Color(0F, 0F, 1F);
    private Color BLACK = new Color(0F, 0F, 0F);

    private SpriteRenderer spriteRenderer;
    private PlayerNumber currentWinner;

    // Use this for initialization.  Using Awake instead of Start, because the controller will cause a call
    // to onActivated before Start() is called.
    void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {

    }

    public PlayerNumber getWinner() {
        return this.currentWinner;
    }

    public void onUpdated(PlayerNumber playerNumber)
    {
        this.currentWinner = playerNumber;

        if (PlayerNumber.PLAYER1 == playerNumber) {
            spriteRenderer.color = YELLOW;
        }
        else if (PlayerNumber.PLAYER2 == playerNumber) {
            spriteRenderer.color = BLUE;
        }
        else {
            spriteRenderer.color = BLACK;
        }
    }

}
