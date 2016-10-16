using System;
using UnityEngine;

public class AbstractAvatarController : MonoBehaviour {

    private float yPos = 0;

    void Start () {
        transform.position = new Vector3(getInitialX(), yPos, 0);
    }

    protected virtual float getInitialX() {
        throw new Exception("getInitialX must be overridden");
    }

    public void setYPos(float yPos) {
        this.yPos = yPos;
    }
    public void onSideChanged() {
        transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
    }

}
