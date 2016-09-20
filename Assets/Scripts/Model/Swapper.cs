using System;

public class Swapper : AbstractBoardObject
{
    public Swapper(AbstractBoardObject output) : base() {
        AddOutput(output);
        output.AddInput(this);
    }

    protected override PlayerNumber translatePlayerNumberForOutput(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return PlayerNumber.PLAYER2;
        }

        return PlayerNumber.PLAYER1;
    }

}

