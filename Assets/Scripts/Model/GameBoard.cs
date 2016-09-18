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
public delegate void TargetSummaryActivatedEventHandler(GameBoard sender, EventArgs e);
public delegate void TargetSummaryDeactivatedEventHandler(GameBoard sender, EventArgs e);

public class GameBoard
{
    public event TargetSummaryActivatedEventHandler targetSummaryActivated;
    public event TargetSummaryDeactivatedEventHandler targetSummaryDeactivated;

    private const int ROW_COUNT = 12;

    private List<Target> targets = new List<Target>();
    private AbstractBoardObject[,] player1Board = new AbstractBoardObject[3,ROW_COUNT];
    private AbstractBoardObject[,] player2Board = new AbstractBoardObject[3,ROW_COUNT];

	public GameBoard()
	{
        for (int i = 0 ; i < ROW_COUNT ; i++) {
            Target target = new Target();
            target.boardObjectActivated += onTargetActivatedStateChanged;
            target.boardObjectDeactivated += onTargetActivatedStateChanged;
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

    public void onFirePressed(int playerPosition)
    {
        //TODO: Check if this needs to work for both players
        this.player1Board[0, playerPosition].inputActivated(null);
    }

    public void onPlayerRemoved(int playerPosition)
    {
        //TODO: Check if this needs to work for both players
        this.player1Board[0, playerPosition].inputDeactivated(null);
    }

    protected void OnTargetSummaryActivated(EventArgs eventArgs) {
        Debug.Log("OnTargetSummaryActivated.");
        if (targetSummaryActivated != null)
            targetSummaryActivated(this, eventArgs);
    }

    protected void OnTargetSummaryDeactivated(EventArgs eventArgs) {
        Debug.Log("OnTargetSummaryDeactivated.");
        if (targetSummaryDeactivated != null)
            targetSummaryDeactivated(this, eventArgs);
    }

    private void onTargetActivatedStateChanged(BoardObject sender, EventArgs e) {
        int activeCount = 0;
        for (int i = 0 ; i < this.targets.Count ; i++) {
            if (this.targets[i].isActivated()) {
                activeCount = activeCount + 1;
            }
        }

        // TODO: when we have two players, we should indicate who controls the target summary, so the view can 
        // show it as Yellow, Black or Blue.  For now, just activate / de-activate
        if (activeCount > ROW_COUNT / 2) {
            OnTargetSummaryActivated(EventArgs.Empty);
        }
        else {
            OnTargetSummaryDeactivated(EventArgs.Empty);
        }
    }

}

