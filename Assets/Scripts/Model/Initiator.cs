using System;
using System.Collections.Generic;
using UnityEngine;

public class Initiator : AbstractBoardObject
{
    private const float SWAPPER_ZAP_LIFETIME = 8f;

    public Initiator(AbstractBoardObject output) : base() {
        AddOutput(output);
        output.AddInput(this);
    }

    public override void inputDeactivated(Model model, BoardObject input) {
        // don't want to deactivate just yet, so use a co-routine to delay the action 
        model.StartCoroutine(executeInputDeactivated(model, input));
    }

    private IEnumerator<WaitForSeconds> executeInputDeactivated(Model model, BoardObject input) {

        yield return new WaitForSeconds(SWAPPER_ZAP_LIFETIME);

        Debug.Log("swapper disabled");

        base.inputDeactivated(model, input);
    }


}

