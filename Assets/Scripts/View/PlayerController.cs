using System;
using UnityEngine;

public class PlayerController : ZapController
{
    private float period = 0.15f;
    private float nextActionTime = 0.0f;

    // this is to ensure that the user doesn't fire all their zaps in one go!  Essentially a zap can only be fired
    // after a FixedUpdate has happened since they released the fire button.
    private bool fireKeyDown  = true;

    private float initialX = -5.26f;
    private float moveByOnActivate = 0.38f;

    protected override float getX() {
        return initialX;
    }

    public override void reset()
    {
        base.reset();
        nextActionTime = Time.time;
    }

    // called before performing any physics calculations
    void FixedUpdate() {
        if (this.gamePhase == GamePhase.CHOOSE_COLOUR) {
            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + period;

                if (Input.GetKey(KeyCode.LeftArrow)) {
                    OnSideChangeRequested(new SideChangeRequestedEventArgs(Side.LEFT));
                }
                else if (Input.GetKey(KeyCode.RightArrow)) {
                    OnSideChangeRequested(new SideChangeRequestedEventArgs(Side.RIGHT));
                }
            }
            return;
        }

        if (this.gamePhase == GamePhase.MAIN_GAME) {
            if (!fireKeyDown && Input.GetKey(KeyCode.Space)) {
                // issue a 'fire' event
                fireKeyDown = true;
                OnFirePressed(new FirePressedEventArgs(PlayerNumber.PLAYER1));
            }

            if (!Input.GetKey(KeyCode.Space)) {
                fireKeyDown = false;
            }

            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + period;

                if (Input.GetKey(KeyCode.UpArrow)) {
                    OnPlayerMoveRequested(new PlayerMoveRequestedEventArgs(PlayerNumber.PLAYER1, Direction.UP));
                }
                else if (Input.GetKey(KeyCode.DownArrow)) {
                    OnPlayerMoveRequested(new PlayerMoveRequestedEventArgs(PlayerNumber.PLAYER1, Direction.DOWN));
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

