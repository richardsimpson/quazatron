    using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBoardObjectController : MonoBehaviour
{
    private List<AbstractBoardObjectController> outputs = new List<AbstractBoardObjectController>();

    public AbstractBoardObjectController() {

    }

    public void AddOutput(AbstractBoardObjectController output) {
        this.outputs.Add(output);
    }

    public List<AbstractBoardObjectController> getOutputs() {
        return this.outputs;
    }

    public virtual void onActivated() {
        
    }
}

