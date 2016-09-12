using System;

public class GameBoard
{
    private Target[] targets = new Target[12];
    private BoardObject[] inputs = new BoardObject[12];

	public GameBoard()
	{
        for (int i = 0 ; i < this.targets.Length ; i++) {
            this.targets[i] = new Target();
        }

        // setup the board.  For now, just create a bunch of wires, one to each target
        for (int i = 0 ; i < this.inputs.Length ; i++) {
            this.inputs[i] = new Wire(this.targets[i]);
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

