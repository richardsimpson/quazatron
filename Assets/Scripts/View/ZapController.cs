using System;
using UnityEngine;

public delegate void PlayerMoveRequestedEventHandler(object sender, PlayerMoveRequestedEventArgs e);
public delegate void FirePressedEventHandler(object sender, FirePressedEventArgs e);

public class FirePressedEventArgs : EventArgs
{
    public PlayerNumber playerNumber;

    public FirePressedEventArgs(PlayerNumber playerNumber)
    {
        this.playerNumber = playerNumber;
    }    

}

public class ZapController : MonoBehaviour
{
    public event PlayerMoveRequestedEventHandler playerMoveRequested;
    public event FirePressedEventHandler firePressed;

    private const float INITIAL_Y = 3f;
    private const float MOVE_BY_Y = 0.60f;

    protected int playerPosition = 0;

    protected virtual float getInitialX() {
        throw new Exception("getInitialX must be overridden");
    }

    public virtual void reset()
    {
        transform.position = new Vector3(getInitialX(), INITIAL_Y, 0);
    }

    // Use this for initialization
    void Start () {
        reset();
    }

    protected void OnPlayerMoveRequested(PlayerMoveRequestedEventArgs eventArgs) {
        Debug.Log("OnPlayerMoveRequested: " + eventArgs.direction);
        if (playerMoveRequested != null)
            playerMoveRequested(this, eventArgs);
    }

    protected void OnFirePressed(FirePressedEventArgs eventArgs) {
        Debug.Log("OnFirePressed.");
        if (firePressed != null)
            firePressed(this, eventArgs);
    }

    public void onPlayerMoved(int position)
    {
        this.playerPosition = position;
        transform.position = new Vector3(getInitialX(), INITIAL_Y - (MOVE_BY_Y*position));
    }

    public int getPlayerPosition() {
        return this.playerPosition;
    }

    public void setActive(bool active) {
        this.gameObject.SetActive(active);
    }
}

