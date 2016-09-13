using System;
using System.Collections.Generic;
using UnityEngine;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class View : MonoBehaviour
{
    private const float INITIAL_Y = 4f;
    private const float Y_INCREMENT = 0.75f;

    private const float LIVES_X = -8f;
    private const float LIVES_INITIAL_Y = 4f;

    private const int MAX_INPUTS = 12;

    public TargetController targetPrefab;
    public WireController wirePrefab;
    public PlayerController playerPrefab;

    private List<TargetController> targets = new List<TargetController>();
    private List<WireController> inputs = new List<WireController>();

    private PlayerController player;
    private List<PlayerController> playerLives = new List<PlayerController>();
    private List<PlayerController> oldPlayers = new List<PlayerController>();

    public void init() {
        constructTargetViews();
        constructInputViews();
    }

    private void constructTargetViews() {
        float yPos = INITIAL_Y;

        for (int index = 0 ; index < MAX_INPUTS ; index++) {
            TargetController newTarget = Instantiate(targetPrefab);
            newTarget.transform.position = new Vector3(0, yPos, 0);

            this.targets.Add(newTarget);

            yPos = yPos - Y_INCREMENT;
        }
    }

    private WireController constructWire(int column, float yPos) {
        WireController result = Instantiate(wirePrefab);

        float colZeroXPos = this.targets[0].transform.position.x - (this.targets[0].transform.localScale.x/2)
            - (result.transform.localScale.x/2);

        float wireWidth = result.transform.localScale.x;

        result.transform.position = new Vector3(colZeroXPos-(wireWidth*column), yPos, 0);
        return result;
    }

    private void constructInputViews() {
        float yPos = INITIAL_Y;

        for (int index = 0 ; index < MAX_INPUTS ; index++) {
            WireController wire3 = constructWire(0, yPos);
            WireController wire2 = constructWire(1, yPos);
            WireController wire1 = constructWire(2, yPos);

            wire1.AddOutput(wire2);
            wire2.AddOutput(wire3);
            wire3.AddOutput(this.targets[index]);

            this.inputs.Add(wire1);

            yPos = yPos - Y_INCREMENT;
        }
    }

    public List<TargetController> getTargets()
    {
        return this.targets;
    }

    public List<WireController> getInputs()
    {
        return this.inputs;
    }

    public PlayerController createPlayer()
    {
        this.player = Instantiate<PlayerController>(playerPrefab); 
        float posX = this.targets[0].transform.position.x - (this.targets[0].transform.localScale.x/2);
        posX = posX - this.inputs[0].transform.localScale.x * 3 - 50;
        return this.player;
    }

    public List<PlayerController> createLives(int numberOfLives)
    {
        float posY = LIVES_INITIAL_Y;

        for (int i = 1 ; i < numberOfLives ; i++) {
            PlayerController player = Instantiate<PlayerController>(playerPrefab);
            player.transform.position = new Vector3(LIVES_X, posY, 0);
            posY = posY - player.transform.localScale.y;
            player.enabled = false;
            playerLives.Add(player);
        }

        return this.playerLives;
    }

    public void onPlayerMoved(int position)
    {
        this.player.onPlayerMoved(position);
    }

    public void onBoardObjectActivated(AbstractBoardObjectController boardObjectController)
    {
        boardObjectController.onActivated();
    }

    public void onZapFired()
    {
        // move the player one space to the right, 
        Transform t = this.player.transform;
        t.position += new Vector3(t.localScale.x, 0, 0);

        // disable it's script.
        this.player.enabled = false;

        // move it to the list of old players
        oldPlayers.Add(this.player);

        // take the last player object from the Lives list and 'reset' it to be at the initial player position (first wire).
        if (this.playerLives.Count > 0) {
            this.player = this.playerLives[this.playerLives.Count-1];
            this.playerLives.RemoveAt(this.playerLives.Count-1);
            this.player.reset();

            // then enable it's script.
            this.player.enabled = true;
        }
    }
}