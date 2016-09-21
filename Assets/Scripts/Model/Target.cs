using System;
using System.Collections.Generic;
using UnityEngine;

public class Target : AbstractBoardObject
{
    public Target() : base() {
    }

    // unlike all other board objects, the state of the targets is not just active / inactive.
    // instead, we need to identify which inputs are active, and which 'colour' they are.   To do this,
    // we need to remember the 'playerNumber' of the input as it was when it was activated

    // Then we need to raise a new event, I think, instead of boardObjectActivated(BoardObjectActivatedEventArgs)
    // and boardObjectDeactivated(EventArgs), listened to by the Controller and GameBoard, 

    private Dictionary<BoardObject, PlayerNumber> activatedInputs = new Dictionary<BoardObject, PlayerNumber>();

    private PlayerNumber controllingPlayer = PlayerNumber.NEITHER;

    private void recalculateControllingPlayer() {
        bool player1Input = this.activatedInputs.ContainsValue(PlayerNumber.PLAYER1);
        bool player2Input = this.activatedInputs.ContainsValue(PlayerNumber.PLAYER2);

        if (player1Input && !player2Input) {
            this.controllingPlayer = PlayerNumber.PLAYER1;
            return;
        }

        if (!player1Input && player2Input) {
            this.controllingPlayer = PlayerNumber.PLAYER2;
            return;
        }

        if (player1Input && player2Input) {
            this.controllingPlayer = PlayerNumber.BOTH;
            return;
        }

        // only other possible situation is that there are no active inputs.  The previous controlling
        // player have been either PLAYER1 or PLAYER2 (it cannot be BOTH), so we just leave the
        // controlling player as is
    }

    private void fireActiveEventIfNecessary() {
        recalculateControllingPlayer();

        if (PlayerNumber.NEITHER != this.controllingPlayer) {
            OnBoardObjectActivated(new BoardObjectActivatedEventArgs(this.controllingPlayer));
        }
    }

    public override void inputActivated(BoardObject input, PlayerNumber playerNumber) {
        activatedInputs.Add(input, playerNumber);
        fireActiveEventIfNecessary();
    }

    public override void inputDeactivated(BoardObject input) {
        activatedInputs.Remove(input);
        fireActiveEventIfNecessary();
    }

    public PlayerNumber getControllingPlayer() {
        return this.controllingPlayer;
    }

    public void setControllingPlayer(PlayerNumber playerNumber) {
        this.controllingPlayer = playerNumber;
        OnBoardObjectActivated(new BoardObjectActivatedEventArgs(this.controllingPlayer));
    }
}

