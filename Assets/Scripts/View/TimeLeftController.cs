using System;
using UnityEngine;
using UnityEngine.UI;

public delegate void GamePhaseChangeEventHandler(TimeLeftController sender, GamePhaseChangeEventArgs e);

public enum GamePhase {
    CHOOSE_COLOUR,
    MAIN_GAME,
    GAME_OVER
};

public class GamePhaseChangeEventArgs : EventArgs {

    public GamePhase gamePhase;

    public GamePhaseChangeEventArgs(GamePhase gamePhase) {
        this.gamePhase = gamePhase;
    }
}
   
public class TimeLeftController : MonoBehaviour {

    // time for selecting colour, as measured in the original game: 8.75 seconds
    // time for main game, as measured in the original game: 13.75 seconds
    //
    // Given that we want both stages to use a counter from 99 to 0, this means that we need
    // each number in the countdown to be:
    //
    //      selecting colour: 8.75 / 100 = 0.0875 seconds
    //      main game: 15.00 / 100 = 0.15 seconds

    private float chooseColourPeriod = 0.0875f;
    private float mainGamePeriod = 0.15f;
    private float nextActionTime = 0.0f;

    private String chooseColourPrefix = "Choose Colour: ";
    private String mainGamePrefix = "Time Left: ";

    private GamePhase phase = GamePhase.CHOOSE_COLOUR;

    private float period;
    private String prefix;

    private int count;
    private Text textComponent;

    public event GamePhaseChangeEventHandler gamePhaseChange;

	// Use this for initialization
	void Start () {
        this.count = 100;
        this.textComponent = GetComponent<Text>();

        this.period = chooseColourPeriod;
        this.prefix = chooseColourPrefix;

    }
	
    private void onGamePhaseChange(GamePhaseChangeEventArgs eventArgs) {
        Debug.Log("onGamePhaseChange: " + eventArgs);

        this.phase = eventArgs.gamePhase;

        if (gamePhaseChange != null)
            gamePhaseChange(this, eventArgs);
    }

	void FixedUpdate () {
        if ((count == 0) &&(this.phase == GamePhase.MAIN_GAME)) {
            onGamePhaseChange(new GamePhaseChangeEventArgs(GamePhase.GAME_OVER));
        }

        if ((count == 0) &&(this.phase == GamePhase.CHOOSE_COLOUR)) {

            // NEED to pass the game phase to the EnemyController and PlayerController, so that they don't fire zaps until it is allowed.
            // Throw an event that the view listens to, that can then call methods on the player and enemy controllers, so they know
            // how to behave.
            onGamePhaseChange(new GamePhaseChangeEventArgs(GamePhase.MAIN_GAME));
            this.period = this.mainGamePeriod;
            this.prefix = this.mainGamePrefix;
            this.count = 100;
        }

        if (count > 0) {
            if (Time.time > nextActionTime) {
                this.count = this.count - 1;
                this.textComponent.text = this.prefix + count;

                nextActionTime = Time.time + period;

                // If player pressed 'space', terminate the choose colour mode
                if ((this.phase == GamePhase.CHOOSE_COLOUR) && (Input.GetKey(KeyCode.Space))) {
                    count = 0;
                }
            }
        }
	}
}
