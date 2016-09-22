using System;
using UnityEngine;
using UnityEngine.UI;

public delegate void GameOverEventHandler(TimeLeftController sender, EventArgs e);

public class TimeLeftController : MonoBehaviour {

    // time for selecting colour, as measured in the original game: 8.75 seconds
    // time for main game, as measured in the original game: 13.75 seconds
    //
    // Given that we want both stages to use a counter from 99 to 0, this means that we need
    // each number in the countdown to be:
    //
    //      selecting colour: 8.75 / 100 = 0.0875 seconds
    //      main game: 15.00 / 100 = 0.15 seconds

    private float mainGamePeriod = 0.15f;
    private float nextActionTime = 0.0f;

    private int count;
    private Text textComponent;

    public event GameOverEventHandler gameOver;

	// Use this for initialization
	void Start () {
        this.count = 100;
        this.textComponent = GetComponent<Text>();
	}
	
    private void onGameOver(EventArgs eventArgs) {
        Debug.Log("onGameOver.");
        if (gameOver != null)
            gameOver(this, eventArgs);
    }

	void FixedUpdate () {
        if (count == 0) {
            onGameOver(EventArgs.Empty);
        }

        if (count > 0) {
            if (Time.time > nextActionTime) {
                this.count = this.count - 1;
                this.textComponent.text = "Time Left: " + count;

                nextActionTime = Time.time + mainGamePeriod;
            }
        }
	}
}
