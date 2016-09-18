using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void ZapFiredEventHandler(Model sender, EventArgs e);
public delegate void ZapExpiredEventHandler(Model sender, ZapExpiredEventArgs e);

public class ZapExpiredEventArgs : EventArgs
{
    public int playerPosition;

    public ZapExpiredEventArgs(int playerPosition)
    {
        this.playerPosition = playerPosition;
    }    

}

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class Model : MonoBehaviour
{
    private const int NUMBER_OF_LIVES = 8;

    public event ZapFiredEventHandler zapFired;
    public event ZapExpiredEventHandler zapExpired;

    // Data
    private GameBoard gameBoard;
    private Player currentPlayer;
    private Dictionary<int, OldPlayer> oldPlayers = new Dictionary<int, OldPlayer>();

    public void init() {
        this.gameBoard = new GameBoard();
        this.currentPlayer = new Player();
    }

    public List<Target> getTargets() {
        return this.gameBoard.getTargets();
    }

    public BoardObject[,] getPlayer1Board() {
        return this.gameBoard.getPlayer1Board();
    }

    public BoardObject[,] getPlayer2Board() {
        return this.gameBoard.getPlayer2Board();
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

    private void onZapExpired(ZapExpiredEventArgs eventArgs) {
        Debug.Log("onZapExpired.");
        if (zapExpired != null)
            zapExpired(this, eventArgs);
    }

    public void onFirePressed()
    {
        int playerPosition = this.currentPlayer.getPlayerPosition();

        // don't allow a zap to be fired if there is one in the current player position already
        if (this.oldPlayers.ContainsKey(playerPosition)) {
            return;
        }

        // don't allow a zap to be fired if there is a terminator in the first column of the current row.
        if (this.getPlayer1Board()[0, playerPosition] is Terminator) {
            return;
        }

        // Construct a new OldPlayer object
        OldPlayer oldPlayer = new OldPlayer(playerPosition);

        // Add OldPlayer to a new 'old players' dictionary, and start a co-routine to remove them in the future
        this.oldPlayers.Add(playerPosition, oldPlayer);
        StartCoroutine(removePlayer(oldPlayer));

        // activate the current 'player', to light up the wire, etc
        this.gameBoard.onFirePressed(this.currentPlayer.getPlayerPosition());

        // Reset the 'currentPlayer' object.
        this.currentPlayer.reset();

        // Fire a 'zapFired' event.  In the view this populates 'old player', and moves the zap onto the grid.
        onZapFired(EventArgs.Empty);
    }

    private IEnumerator<WaitForSeconds> removePlayer(OldPlayer oldPlayer) {

        yield return new WaitForSeconds(5);

        Debug.Log("player removed");

        this.oldPlayers.Remove(oldPlayer.getPlayerPosition());

        this.gameBoard.onPlayerRemoved(oldPlayer.getPlayerPosition());
        onZapExpired(new ZapExpiredEventArgs(oldPlayer.getPlayerPosition()));
    }

    public void onPlayerMoveRequested(Direction direction)
    {
        this.currentPlayer.onPlayerMoveRequested(direction);
    }

    public int getNumberOfLives() {
        return NUMBER_OF_LIVES;
    }
}