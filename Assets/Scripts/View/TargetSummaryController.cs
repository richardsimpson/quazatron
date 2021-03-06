﻿using System;
using UnityEngine;

public class TargetSummaryController : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private PlayerNumber currentWinner;

    // Use this for initialization.  Using Awake instead of Start, because the controller will cause a call
    // to onActivated before Start() is called.
    void Awake () {
        SpriteRenderer[] comps = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].sprite.name == "Target") {
                spriteRenderer = comps[i];
            }
        }

        if (spriteRenderer == null) {
            throw new Exception("Cannot locate correct SpriteRenderer component");
        }
    }

    // Update is called once per frame
    void Update () {

    }

    public PlayerNumber getWinner() {
        return this.currentWinner;
    }

    public void onUpdated(PlayerNumber playerNumber, Side player1side)
    {
        this.currentWinner = playerNumber;

        Color colour = ViewConstants.getColourForPlayerNumberAndSide(playerNumber, player1side);

        spriteRenderer.color = colour;
    }

}
