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
    private Target[] targets = new Target[12];
    private List<BoardObject> inputs = new List<BoardObject>();

	public GameBoard()
	{
        for (int i = 0 ; i < this.targets.Length ; i++) {
            this.targets[i] = new Target();
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

        // TODO: change View.cs so that it can create these items

        // column 3 first:
        BoardObject col3row1 = new Wire(this.targets[0]);
        BoardObject col3row2 = new Wire(this.targets[1]);
        BoardObject col3row3 = null;
        BoardObject col3row4 = new Wire(this.targets[3]);
        BoardObject col3row5 = new Wire(this.targets[4]);
        BoardObject col3row6 = new Swapper(this.targets[5]);
        BoardObject col3row7 = null;
        BoardObject col3row8 = null;
        BoardObject col3row9 = null;
        BoardObject col3row10 = new Connector(this.targets[9]);
        BoardObject col3row11 = null;
        BoardObject col3row12 = new Wire(this.targets[11]);

        BoardObject col2row1 = new Wire(col3row1);
        BoardObject col2row2 = null;
        List<BoardObject> outputs = new List<BoardObject>();
        outputs.Add(col3row2);
        outputs.Add(col3row4);
        BoardObject col2row3 = new Connector(outputs);
        BoardObject col2row4 = null;
        BoardObject col2row5 = new Initiator(col3row5);
        BoardObject col2row6 = new Wire(col3row6);
        BoardObject col2row7 = new Terminator();
        BoardObject col2row8 = null;
        BoardObject col2row9 = new Connector(col3row10);
        BoardObject col2row10 = null;
        BoardObject col2row11 = new Wire(col3row10);
        BoardObject col2row12 = new Wire(col3row12);

        BoardObject col1row1 = new Wire(col2row1);
        BoardObject col1row2 = new Terminator();
        BoardObject col1row3 = new Wire(col2row3);
        BoardObject col1row4 = new Terminator();
        BoardObject col1row5 = new Wire(col2row5);
        BoardObject col1row6 = new Wire(col2row6);
        BoardObject col1row7 = new Wire(col2row7);
        BoardObject col1row8 = new Wire(col2row9);
        BoardObject col1row9 = new Terminator();
        BoardObject col1row10 = new Wire(col2row9);
        BoardObject col1row11 = new Wire(col2row11);
        BoardObject col1row12 = new Wire(col2row12);

        this.inputs.Add(col1row1);
        this.inputs.Add(col1row2);
        this.inputs.Add(col1row3);
        this.inputs.Add(col1row4);
        this.inputs.Add(col1row5);
        this.inputs.Add(col1row6);
        this.inputs.Add(col1row7);
        this.inputs.Add(col1row8);
        this.inputs.Add(col1row9);
        this.inputs.Add(col1row10);
        this.inputs.Add(col1row11);
        this.inputs.Add(col1row12);
	}

    public Target[] getTargets() {
        return this.targets;
    }

    public List<BoardObject> getInputs() {
        return this.inputs;
    }

    public void onFirePressed(int playerPosition)
    {
        this.inputs[playerPosition].inputActivated(null);
    }
}

