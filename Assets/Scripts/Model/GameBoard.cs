using System;
using System.Collections.Generic;
using UnityEngine;

// Game Board Description
//
// Each side of the game board follows this pattern:
//
// P1--2--3--[] 
//
// Where:
//     P is the column that the player moved up and down in
//     1,2,3 are the locations of board objects (splitters, terminators, etc
//     [] is the central column
//
// When firing a 'zap', the zap is moved into column '1'.
//
// Note that if there is a terminator in '1', then the player cannot fire into that row.
// Note also that a player cannot fire into a row that alrady has an active zap.

// NOTE: This will ultimately indicate which player is winning, via an enum (just like wires), but for now the event is empty.
public delegate void TargetSummaryUpdatedEventHandler(GameBoard sender, TargetSummaryUpdatedEventArgs e);

public class TargetSummaryUpdatedEventArgs : EventArgs
{
    public PlayerNumber playerNumber;

    public TargetSummaryUpdatedEventArgs(PlayerNumber playerNumber)
    {
        this.playerNumber = playerNumber;
    }    
}

public class GameBoard
{
    public event TargetSummaryUpdatedEventHandler targetSummaryUpdated;

    private const int ROW_COUNT = 12;

    private List<Target> targets = new List<Target>();
    private AbstractBoardObject[,] player1Board = new AbstractBoardObject[3,ROW_COUNT];
    private AbstractBoardObject[,] player2Board = new AbstractBoardObject[3,ROW_COUNT];

	public GameBoard()
	{
        for (int i = 0 ; i < ROW_COUNT ; i++) {
            Target target = new Target();
            target.boardObjectActivated += onTargetActivatedStateChanged;
            this.targets.Add(target);
        }

        createBoard(this.player1Board);
        createBoard(this.player2Board);
	}

    private void createBoard(AbstractBoardObject[,] board) {
        // setup this board:
        //        1      2      3
        //
        //     1  ---------------------
        //     2  <      --------------
        //     3  -------|
        //     4  <      --------------
        //     5  ------->-------------
        //     6  --------------o------
        //     7  -------<
        //     8  -------
        //     9  <      |------
        //    10  -------       |------ 
        //    11  --------------
        //    12  ---------------------

        // column 3 first:
        board[2, 0] = new Wire(this.targets[0]);
        board[2, 1] = new Wire(this.targets[1]);
        board[2, 2] = null;
        board[2, 3] = new Wire(this.targets[3]);
        board[2, 4] = new Wire(this.targets[4]);
        board[2, 5] = new Swapper(this.targets[5]);
        board[2, 6] = null;
        board[2, 7] = null;
        board[2, 8] = null;
        board[2, 9] = new Connector(this.targets[9]);
        board[2, 10] = null;
        board[2, 11] = new Wire(this.targets[11]);

        board[1, 0] = new Wire(board[2, 0]);
        board[1, 1] = null;
        List<AbstractBoardObject> outputs = new List<AbstractBoardObject>();
        outputs.Add(board[2, 1]);
        outputs.Add(board[2, 3]);
        board[1, 2] = new Connector(outputs);
        board[1, 3] = null;
        board[1, 4] = new Initiator(board[2, 4]);
        board[1, 5] = new Wire(board[2, 5]);
        board[1, 6] = new Terminator();
        board[1, 7] = null;
        board[1, 8] = new Connector(board[2, 9]);
        board[1, 9] = null;
        board[1, 10] = new Wire(board[2, 9]);
        board[1, 11] = new Wire(board[2, 11]);

        board[0, 0] = new Wire(board[1, 0]);
        board[0, 1] = new Terminator();
        board[0, 2] = new Wire(board[1, 2]);
        board[0, 3] = new Terminator();
        board[0, 4] = new Wire(board[1, 4]);
        board[0, 5] = new Wire(board[1, 5]);
        board[0, 6] = new Wire(board[1, 6]);
        board[0, 7] = new Wire(board[1, 8]);
        board[0, 8] = new Terminator();
        board[0, 9] = new Wire(board[1, 8]);
        board[0, 10] = new Wire(board[1, 10]);
        board[0, 11] = new Wire(board[1, 11]);
    }

    public List<Target> getTargets() {
        return this.targets;
    }

    public BoardObject[,] getPlayer1Board() {
        return this.player1Board;
    }

    public BoardObject[,] getPlayer2Board() {
        return this.player2Board;
    }

    private BoardObject[,] getBoardForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return this.player1Board;
        }

        return player2Board;
    }

    public void onFirePressed(PlayerNumber playerNumber, int playerPosition)
    {
        BoardObject[,] board = getBoardForPlayerNumber(playerNumber);
        board[0, playerPosition].inputActivated(null, playerNumber);
    }

    public void onPlayerRemoved(PlayerNumber playerNumber, int playerPosition)
    {
        BoardObject[,] board = getBoardForPlayerNumber(playerNumber);
        board[0, playerPosition].inputDeactivated(null);
    }

    protected void OnTargetSummaryUpdated(TargetSummaryUpdatedEventArgs eventArgs) {
        Debug.Log("OnTargetSummaryUpdated.");
        if (targetSummaryUpdated != null)
            targetSummaryUpdated(this, eventArgs);
    }

    private void recalculateTargetSummaryState() {
        int player1Targets = 0;
        int player2Targets = 0;

        for (int i = 0 ; i < this.targets.Count ; i++) {
            PlayerNumber controllingPlayer = this.targets[i].getControllingPlayer();
            if (PlayerNumber.PLAYER1 == controllingPlayer) {
                player1Targets = player1Targets + 1;
            }
            else if (PlayerNumber.PLAYER2 == controllingPlayer) {
                player2Targets = player2Targets + 1;
            }
        }

        if (player1Targets > player2Targets) {
            OnTargetSummaryUpdated(new TargetSummaryUpdatedEventArgs(PlayerNumber.PLAYER1));
        }
        else if (player1Targets < player2Targets) {
            OnTargetSummaryUpdated(new TargetSummaryUpdatedEventArgs(PlayerNumber.PLAYER2));
        }
        else {
            OnTargetSummaryUpdated(new TargetSummaryUpdatedEventArgs(PlayerNumber.NEITHER));
        }
    }

    private void onTargetActivatedStateChanged(BoardObject sender, BoardObjectActivatedEventArgs e) {
        recalculateTargetSummaryState();
    }

    // TODO: Is this used?
    private void onTargetDeactivatedStateChanged(BoardObject sender, EventArgs e) {
        recalculateTargetSummaryState();
    }

}

