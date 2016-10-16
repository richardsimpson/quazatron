using System;
using UnityEngine;

public delegate void SideChangedRequestedEventHandler(object sender, SideChangeRequestedEventArgs e);

public enum Side {
    LEFT,
    RIGHT
}

public class SideChangeRequestedEventArgs : EventArgs
{
    public Side side;

    public SideChangeRequestedEventArgs(Side side)
    {
        this.side = side;
    }

}

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

    // initial z is -1, so we are guaranteed that the player icons appear in fron of all other components
    private const float INITIAL_Z = -1.0f;

    protected int playerPosition = 0;
    protected GamePhase gamePhase = GamePhase.CHOOSE_COLOUR;

    public event SideChangedRequestedEventHandler sideChangeRequested;

    protected virtual float getX() {
        throw new Exception("getX must be overridden");
    }

    public virtual void reset()
    {
        transform.position = new Vector3(getX(), ViewConstants.INITIAL_Y, INITIAL_Z);
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
        transform.position = new Vector3(getX(), ViewConstants.INITIAL_Y - (ViewConstants.Y_INCREMENT*position));
    }

    public int getPlayerPosition() {
        return this.playerPosition;
    }

    public void setActive(bool active) {
        this.gameObject.SetActive(active);
    }

    public void onGamePhaseChange(GamePhase gamePhase)
    {
        this.gamePhase = gamePhase;
    }

    protected void OnSideChangeRequested(SideChangeRequestedEventArgs eventArgs) {
        Debug.Log("OnSideChangeRequested: " + eventArgs.side);
        if (sideChangeRequested != null)
            sideChangeRequested(this, eventArgs);
    }

    public virtual void onSideChanged(Side side) {
        throw new Exception("onSideChanged must be overridden");
    }

    public virtual void moveOnZapFired() {
        throw new Exception("moveOnZapFired must be overridden");
    }
}

