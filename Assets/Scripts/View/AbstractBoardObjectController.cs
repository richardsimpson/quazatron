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

    public void AddOutputs(List<AbstractBoardObjectController> outputs) {
        for (int i = 0 ; i < outputs.Count ; i++) {
            this.outputs.Add(outputs[i]);
        }
    }

    public List<AbstractBoardObjectController> getOutputs() {
        return this.outputs;
    }

    public virtual void onActivated() {
        
    }
}

