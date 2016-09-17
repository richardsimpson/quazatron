using System;
using System.Collections.Generic;

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

public class GameBoard
{
    private const int ROW_COUNT = 12;

    private List<Target> targets = new List<Target>();
    private AbstractBoardObject[,] board = new AbstractBoardObject[3,ROW_COUNT];

	public GameBoard()
	{
        for (int i = 0 ; i < ROW_COUNT ; i++) {
            this.targets.Add(new Target());
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
}

