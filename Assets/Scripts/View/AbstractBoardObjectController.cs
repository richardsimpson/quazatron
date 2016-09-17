using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBoardObjectController : MonoBehaviour
{
    protected Color YELLOW = new Color(1F, 1F, 0F);

    public AbstractBoardObjectController() {

    }

    public virtual void onActivated() {
        
    }
}

