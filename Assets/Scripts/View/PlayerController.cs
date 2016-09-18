using System;
using UnityEngine;

public delegate void PlayerMoveRequestedEventHandler(object sender, PlayerMoveRequestedEventArgs e);
public delegate void FirePressedEventHandler(object sender, EventArgs e);

public class PlayerController : MonoBehaviour
{
    public event PlayerMoveRequestedEventHandler playerMoveRequested;
    public event FirePressedEventHandler firePressed;

    private float period = 0.15f;

    private float nextActionTime = 0.0f;

    // this is to ensure that the user doesn't fire all their zaps in one go!  Essentially a zap can only be fired
    // after a FixedUpdate has happened since they released the fire button.
    private bool fireKeyDown  = true;

    private const float INITIAL_X = -5.46f;
    private const float INITIAL_Y = 3f;
    private const float MOVE_BY_Y = 0.60f;

    private int playerPosition = 0;

    public void reset()
    {
        transform.position = new Vector3(INITIAL_X, INITIAL_Y, 0);
        nextActionTime = Time.time;

    }

    // Use this for initialization
    void Start () {
        reset();
    }

    private void OnPlayerMoveRequested(PlayerMoveRequestedEventArgs eventArgs) {
        Debug.Log("OnPlayerMoveRequested: " + eventArgs.direction);
        if (playerMoveRequested != null)
            playerMoveRequested(this, eventArgs);
    }

    private void OnFirePressed(EventArgs eventArgs) {
        Debug.Log("OnFirePressed.");
        if (firePressed != null)
            firePressed(this, eventArgs);
    }

    void FixedUpdate() {
        // called before performing any physics calculations
        if (!fireKeyDown && Input.GetKey(KeyCode.Space)) {
            // issue a 'fire' event
            fireKeyDown = true;
            OnFirePressed(EventArgs.Empty);
        }

        if (!Input.GetKey(KeyCode.Space)) {
            fireKeyDown = false;
        }

        if (Time.time > nextActionTime) {
            nextActionTime = Time.time + period;

            if (Input.GetKey(KeyCode.UpArrow)) {
                OnPlayerMoveRequested(new PlayerMoveRequestedEventArgs(Direction.UP));
            }
            else if (Input.GetKey(KeyCode.DownArrow)) {
                OnPlayerMoveRequested(new PlayerMoveRequestedEventArgs(Direction.DOWN));
            }
        }
    }

    public void onPlayerMoved(int position)
    {
        this.playerPosition = position;
        transform.position = new Vector3(INITIAL_X, INITIAL_Y - (MOVE_BY_Y*position));
    }

    public int getPlayerPosition() {
        return this.playerPosition;
    }

    public void setActive(bool active) {
        this.gameObject.SetActive(active);
    }
}

