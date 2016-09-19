using System;
using UnityEngine;

public delegate void PlayerMovedEventHandler(object sender, PlayerMovedEventArgs e);

public class Player
{
    public event PlayerMovedEventHandler playerMoved;

    private const int MIN_PLAYER_POS = 0;
    private const int MAX_PLAYER_POS = 11;

    private readonly PlayerNumber playerNumber;
    private int playerPosition = 0;

    public Player(PlayerNumber playerNumber) {
        this.playerNumber = playerNumber;
    }

    public void reset()
    {
        this.playerPosition = 0;
    }

    public void onPlayerMoveRequested(Direction direction)
    {
        int oldPlayerPosition = this.playerPosition;

        if (direction == Direction.DOWN) {
            this.playerPosition = this.playerPosition + 1;
        }
        else if (direction == Direction.UP) {
            this.playerPosition = this.playerPosition - 1;
        }

        if (this.playerPosition < MIN_PLAYER_POS) {
            this.playerPosition = MAX_PLAYER_POS;
        }

        if (this.playerPosition > MAX_PLAYER_POS) {
            this.playerPosition = MIN_PLAYER_POS;
        }

        if (oldPlayerPosition != this.playerPosition) {
            onPlayerMoved(new PlayerMovedEventArgs(this.playerNumber, this.playerPosition));
        }
    }

    private void onPlayerMoved(PlayerMovedEventArgs eventArgs) {
        Debug.Log("OnPlayerMoved: " + eventArgs.position);
        if (playerMoved != null)
            playerMoved(this, eventArgs);
    }

    public int getPlayerPosition() {
        return this.playerPosition;
    }

}

