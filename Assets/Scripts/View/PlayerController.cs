using System;
using UnityEngine;

public class PlayerController : ZapController
{
    private float period = 0.15f;

    private float nextActionTime = 0.0f;

    // this is to ensure that the user doesn't fire all their zaps in one go!  Essentially a zap can only be fired
    // after a FixedUpdate has happened since they released the fire button.
    private bool fireKeyDown  = true;

    private const float INITIAL_X = -5.46f;

    protected override float getInitialX() {
        return INITIAL_X;
    }

    public override void reset()
    {
        base.reset();
        nextActionTime = Time.time;
    }

    void FixedUpdate() {
        // called before performing any physics calculations
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
    }

}

