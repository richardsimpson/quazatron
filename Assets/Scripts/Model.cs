using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void ZapFiredEventHandler(Model sender, EventArgs e);

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class Model : MonoBehaviour
{
    private const int NUMBER_OF_LIVES = 8;

    public event ZapFiredEventHandler zapFired;

    // Data
    private GameBoard gameBoard;
    private Player currentPlayer;
    private List<OldPlayer> oldPlayers = new List<OldPlayer>();

    public void init() {
        this.gameBoard = new GameBoard();
        this.currentPlayer = new Player();
    }

    public List<Target> getTargets() {
        return this.gameBoard.getTargets();
    }

    public BoardObject[,] getBoard() {
        return this.gameBoard.getBoard();
    }

    public GameBoard getGameBoard() {
        return this.gameBoard;
    }

    public Player getCurrentPlayer() {
        return this.currentPlayer;
    }

    private void onZapFired(EventArgs eventArgs) {
        Debug.Log("onZapFired.");
        if (zapFired != null)
            zapFired(this, eventArgs);
    }

    public void onFirePressed()
    {
        // Construct a new OldPlayer object
        OldPlayer oldPlayer = new OldPlayer(this.currentPlayer.getPlayerPosition());

        // Add OldPlayer to a new 'old players' list.  These objects will eventually need to be removed when they timeout.
        this.oldPlayers.Add(oldPlayer);
        StartCoroutine(removePlayer(oldPlayer));

        // activate the current 'player', to light up the wire, etc
        this.gameBoard.onFirePressed(this.currentPlayer.getPlayerPosition());

        // Reset the 'currentPlayer' object.
        this.currentPlayer.reset();

        // Fire a 'zapFired' event 
        onZapFired(EventArgs.Empty);

        // TODO: When a zap expires, how do we tell the view which one? (add the OldPlayer to the zapFired event, and have
        //       the controller remember this, and it's association with the View's currentPlayer.
    }

    private IEnumerator<WaitForSeconds> removePlayer(OldPlayer oldPlayer) {
        yield return new WaitForSeconds(5);
        Debug.Log("player removed");
        this.gameBoard.onPlayerRemoved(oldPlayer.getPlayerPosition());
    }

    public void onPlayerMoveRequested(Direction direction)
    {
        this.currentPlayer.onPlayerMoveRequested(direction);
    }

    public int getNumberOfLives() {
        return NUMBER_OF_LIVES;
    }
}