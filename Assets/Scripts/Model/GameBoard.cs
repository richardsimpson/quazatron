using System;

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
    private BoardObject[] inputs = new BoardObject[12];

	public GameBoard()
	{
        for (int i = 0 ; i < this.targets.Length ; i++) {
            this.targets[i] = new Target();
        }

        // setup the board.  For now, just create a bunch of wires, three to each target
        for (int i = 0 ; i < this.inputs.Length ; i++) {
            Wire wire3 = new Wire(this.targets[i]);
            Wire wire2 = new Wire(wire3);
            Wire wire1 = new Wire(wire2);
            this.inputs[i] = wire1;
        }
	}

    public Target[] getTargets() {
        return this.targets;
    }

    public BoardObject[] getInputs() {
        return this.inputs;
    }

    public void onFirePressed(int playerPosition)
    {
        this.inputs[playerPosition].inputActivated(null);
    }
}

