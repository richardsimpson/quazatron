using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ZapController
{
    private float period = 0.3f;

    private float nextActionTime = 0.0f;

    private float initialX = 5.22f;
    private float moveByOnActivate = -0.38f;

    private AbstractBoardObjectController[,] boardViews;
    private List<int> validInputs = new List<int>();
    private int currentInputRow = -1;
    private System.Random random = new System.Random();

    protected override float getX() {
        return initialX;
    }

    public override void reset()
    {
        base.reset();
        nextActionTime = Time.time;
    }

    public void setBoard(AbstractBoardObjectController[,] boardViews) {
        this.boardViews = boardViews;

        for (int i = 0 ; i < this.boardViews.GetLength(1) ; i++) {
            AbstractBoardObjectController input = this.boardViews[0, i];
            if (input is TerminatorController) {
                continue;
            }

            validInputs.Add(i);
        }
    }

    private int identifyInputRow() {
        // find a  valid input that is not currently active
        List<int> currentlyValidInputs = new List<int>();

        for (int i = 0 ; i < this.validInputs.Count ; i++) {
            if (!this.boardViews[0,this.validInputs[i]].isActivated()) {
                currentlyValidInputs.Add(this.validInputs[i]);
            }
        }

        if (currentlyValidInputs.Count == 0) {
            return -1;
        }

        int index = this.random.Next(0, currentlyValidInputs.Count);
        return currentlyValidInputs[index];
    }

    void FixedUpdate() {
        if (this.gamePhase == GamePhase.CHOOSE_COLOUR) {
            return;
        }

        if (this.gamePhase == GamePhase.MAIN_GAME) {
            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + period;

                // if not already decided on a move, select one
                if (this.currentInputRow == -1) {
                    this.currentInputRow = identifyInputRow();
                }

                // if still don't know one, there must be no valid input right now.  wait for a while
                if (this.currentInputRow == -1) {
                    // wait 2 seconds, for some zaps to expire
                    nextActionTime = nextActionTime + 2;
                }
                else {
                    if (this.playerPosition > this.currentInputRow) {
                        OnPlayerMoveRequested(new PlayerMoveRequestedEventArgs(PlayerNumber.PLAYER2, Direction.UP));
                    }
                    else if (this.playerPosition < this.currentInputRow) {
                        OnPlayerMoveRequested(new PlayerMoveRequestedEventArgs(PlayerNumber.PLAYER2, Direction.DOWN));
                    }
                    else {
                        this.currentInputRow = -1;
                        OnFirePressed(new FirePressedEventArgs(PlayerNumber.PLAYER2));
                    }
                }
            }
            return;
        }
    }

    public override void onSideChanged(Side side) {
        this.initialX = -this.initialX;
        this.moveByOnActivate = -this.moveByOnActivate;

        transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        transform.Rotate(new Vector3(0, 0, 180));

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (Side.LEFT == side) {
            spriteRenderer.color = ViewConstants.YELLOW;
        }
        else {
            spriteRenderer.color = ViewConstants.BLUE;
        }
    }

    public override void moveOnZapFired() {
        transform.position += new Vector3(moveByOnActivate, 0, 0);
    }
}

