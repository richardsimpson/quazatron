using System;
using UnityEngine;

public class PlayerWallController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer = null;

    public void Awake() {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Target") {
                spriteRenderer = comps[i];
            }
        }

        if (spriteRenderer == null) {
            throw new Exception("Cannot locate SpriteRenderer component");
        }
    }

    public void setOwner(PlayerNumber owner)
    {
        if (owner == PlayerNumber.PLAYER1) {
            spriteRenderer.color = ViewConstants.YELLOW;
        }
        else {
            spriteRenderer.color = ViewConstants.BLUE;
        }
    }
}

