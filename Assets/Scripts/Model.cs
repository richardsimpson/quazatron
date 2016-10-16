using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void ZapFiredEventHandler(Model sender, ZapFiredEventArgs e);
public delegate void ZapExpiredEventHandler(Model sender, ZapExpiredEventArgs e);

public class ZapExpiredEventArgs : EventArgs
{
    public PlayerNumber playerNumber;
    public int playerPosition;

    public ZapExpiredEventArgs(PlayerNumber playerNumber, int playerPosition)
    {
        this.playerNumber = playerNumber;
        this.playerPosition = playerPosition;
    }    

}

public class ZapFiredEventArgs : EventArgs
{
    public PlayerNumber playerNumber;

    public ZapFiredEventArgs(PlayerNumber playerNumber)
    {
        this.playerNumber = playerNumber;
    }    

}

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class Model : MonoBehaviour
{
    private const int PLAYER1_NUMBER_OF_LIVES = 5;
    private const int PLAYER2_NUMBER_OF_LIVES = 8;
    private const float ZAP_LIFETIME = 8.5f;

    public event ZapFiredEventHandler zapFired;
    public event ZapExpiredEventHandler zapExpired;

    // Data
    private GameBoard gameBoard;
    private Player player1;
    private Player player2;
    private Dictionary<PlayerNumber, Dictionary<int, OldPlayer>> oldPlayers = new Dictionary<PlayerNumber, Dictionary<int, OldPlayer>>();

    public void init() {
        this.gameBoard = new GameBoard();
        this.player1 = new Player(PlayerNumber.PLAYER1);
        this.player2 = new Player(PlayerNumber.PLAYER2);
        oldPlayers.Add(PlayerNumber.PLAYER1, new Dictionary<int, OldPlayer>());
        oldPlayers.Add(PlayerNumber.PLAYER2, new Dictionary<int, OldPlayer>());
    }

    public List<Target> getTargets() {
        return this.gameBoard.getTargets();
    }

    public BoardObject[,] getLeftBoard() {
        return this.gameBoard.getLeftBoard();
    }

    public BoardObject[,] getRightBoard() {
        return this.gameBoard.getRightBoard();
    }

    public GameBoard getGameBoard() {
        return this.gameBoard;
    }

    public Player getPlayer1() {
        return this.player1;
    }

    public Player getPlayer2() {
        return this.player2;
    }

    private void onZapFired(ZapFiredEventArgs eventArgs) {
        Debug.Log("onZapFired.");
        if (zapFired != null)
            zapFired(this, eventArgs);
    }

    private void onZapExpired(ZapExpiredEventArgs eventArgs) {
        Debug.Log("onZapExpired.");
        if (zapExpired != null)
            zapExpired(this, eventArgs);
    }

    private Player getPlayerForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return this.player1;
        }

        return this.player2;
    }

    public void onFirePressed(PlayerNumber playerNumber)
    {
        Player player = getPlayerForPlayerNumber(playerNumber);

        int playerPosition = player.getPlayerPosition();
        Dictionary<int, OldPlayer> oldPlayerList = this.oldPlayers[playerNumber];

        // don't allow a zap to be fired if there is one in the current player position already
        if (oldPlayerList.ContainsKey(playerPosition)) {
            return;
        }

        // don't allow a zap to be fired if there is a terminator in the first column of the current row.
        BoardObject[,] board = this.gameBoard.getBoardForPlayerNumber(playerNumber);
        if (board[0, playerPosition] is Terminator) {
            return;
        }

        // Construct a new OldPlayer object
        OldPlayer oldPlayer = new OldPlayer(playerPosition);

        // Add OldPlayer to a new 'old players' dictionary, and start a co-routine to remove them in the future
        oldPlayerList.Add(playerPosition, oldPlayer);
        StartCoroutine(removePlayer(playerNumber, oldPlayer));

        // activate the current 'player', to light up the wire, etc
        this.gameBoard.onFirePressed(playerNumber, player.getPlayerPosition());

        // Reset the 'currentPlayer' object.
        player.reset();

        // Fire a 'zapFired' event.  In the view this populates 'old player', and moves the zap onto the grid.
        onZapFired(new ZapFiredEventArgs(playerNumber));
    }

    private IEnumerator<WaitForSeconds> removePlayer(PlayerNumber playerNumber, OldPlayer oldPlayer) {

        yield return new WaitForSeconds(ZAP_LIFETIME);

        Debug.Log("player removed");

        this.oldPlayers[playerNumber].Remove(oldPlayer.getPlayerPosition());

        // onPlayerRemoved will start deactivating inputs
        this.gameBoard.onPlayerRemoved(this, playerNumber, oldPlayer.getPlayerPosition());
        // zapExpired is listened to by the Controller, so that the View will remove the player object.
        onZapExpired(new ZapExpiredEventArgs(playerNumber, oldPlayer.getPlayerPosition()));
    }

    public void onPlayerMoveRequested(PlayerNumber playerNumber, Direction direction)
    {
        Player player = getPlayerForPlayerNumber(playerNumber);
        player.onPlayerMoveRequested(direction);
    }

    public int getPlayer1NumberOfLives() {
        return PLAYER1_NUMBER_OF_LIVES;
    }

    public int getPlayer2NumberOfLives() {
        return PLAYER2_NUMBER_OF_LIVES;
    }

    public void onSideChangedRequested(Side side)
    {
        this.gameBoard.onSideChangedRequested(side);
    }
}