using System;
using System.Collections.Generic;
using UnityEngine;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class View : MonoBehaviour
{
    private const float LIVES_X = -8f;
    private const float LIVES_INITIAL_Y = 4f;

    private TargetController[] targets;
    private WireController[] inputs;

    public PlayerController playerPrefab;

    private PlayerController player;
    private List<PlayerController> playerLives = new List<PlayerController>();
    private List<PlayerController> oldPlayers = new List<PlayerController>();

    public void setTargets(TargetController[] targetViews)
    {
        this.targets = targetViews;
    }

    public void setInputs(WireController[] inputViews)
    {
        this.inputs = inputViews;
    }

    public PlayerController createPlayer()
    {
        this.player = Instantiate<PlayerController>(playerPrefab);
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

    public void onWireActivated(WireController wireController)
    {
        wireController.onWireActivated();
    }

    public void onTargetActivated(TargetController targetController)
    {
        targetController.onTargetActivated();
    }

    public void onZapFired()
    {
        // move the player one space to the right, 
        Transform t = this.player.transform;
        t.position += new Vector3(t.position.x + t.localScale.x, 0, 0);

        // disable it's script.
        this.player.enabled = false;

        // move it to the list of old players
        oldPlayers.Add(this.player);

        // take the last player object from the Lives list and 'reset' it to be at the initial player position (first wire).
        this.player = this.playerLives[this.playerLives.Count-1];
        this.playerLives.RemoveAt(this.playerLives.Count-1);
        this.player.reset();

        // then enable it's script.
        this.player.enabled = true;
    }
}