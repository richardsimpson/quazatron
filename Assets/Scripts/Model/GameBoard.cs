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
    private AbstractBoardObject[,] board = new AbstractBoardObject[3,ROW_COUNT];

	public GameBoard()
	{
        for (int i = 0 ; i < ROW_COUNT ; i++) {
            Target target = new Target();
            target.boardObjectActivated += onTargetActivatedStateChanged;
            target.boardObjectDeactivated += onTargetActivatedStateChanged;
            this.targets.Add(target);
        }

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
        this.board[2, 0] = new Wire(this.targets[0]);
        this.board[2, 1] = new Wire(this.targets[1]);
        this.board[2, 2] = null;
        this.board[2, 3] = new Wire(this.targets[3]);
        this.board[2, 4] = new Wire(this.targets[4]);
        this.board[2, 5] = new Swapper(this.targets[5]);
        this.board[2, 6] = null;
        this.board[2, 7] = null;
        this.board[2, 8] = null;
        this.board[2, 9] = new Connector(this.targets[9]);
        this.board[2, 10] = null;
        this.board[2, 11] = new Wire(this.targets[11]);

        this.board[1, 0] = new Wire(this.board[2, 0]);
        this.board[1, 1] = null;
        List<AbstractBoardObject> outputs = new List<AbstractBoardObject>();
        outputs.Add(this.board[2, 1]);
        outputs.Add(this.board[2, 3]);
        this.board[1, 2] = new Connector(outputs);
        this.board[1, 3] = null;
        this.board[1, 4] = new Initiator(this.board[2, 4]);
        this.board[1, 5] = new Wire(this.board[2, 5]);
        this.board[1, 6] = new Terminator();
        this.board[1, 7] = null;
        this.board[1, 8] = new Connector(this.board[2, 9]);
        this.board[1, 9] = null;
        this.board[1, 10] = new Wire(this.board[2, 9]);
        this.board[1, 11] = new Wire(this.board[2, 11]);

        this.board[0, 0] = new Wire(this.board[1, 0]);
        this.board[0, 1] = new Terminator();
        this.board[0, 2] = new Wire(this.board[1, 2]);
        this.board[0, 3] = new Terminator();
        this.board[0, 4] = new Wire(this.board[1, 4]);
        this.board[0, 5] = new Wire(this.board[1, 5]);
        this.board[0, 6] = new Wire(this.board[1, 6]);
        this.board[0, 7] = new Wire(this.board[1, 8]);
        this.board[0, 8] = new Terminator();
        this.board[0, 9] = new Wire(this.board[1, 8]);
        this.board[0, 10] = new Wire(this.board[1, 10]);
        this.board[0, 11] = new Wire(this.board[1, 11]);
	}

    public List<Target> getTargets() {
        return this.targets;
    }

    public BoardObject[,] getBoard() {
        return this.board;
    }

    public void onFirePressed(int playerPosition)
    {
        this.board[0, playerPosition].inputActivated(null);
    }

    public void onPlayerRemoved(int playerPosition)
    {
        this.board[0, playerPosition].inputDeactivated(null);
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

        // when we have two players, we should indicate who controls the target summary, so the view can 
        // show it as Yellow, Black or Blue.  For now, just activate / de-activate
        if (activeCount > ROW_COUNT / 2) {
            OnTargetSummaryActivated(EventArgs.Empty);
        }
        else {
            OnTargetSummaryDeactivated(EventArgs.Empty);
        }
    }

}

