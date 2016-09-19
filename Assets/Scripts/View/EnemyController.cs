using System;
using UnityEngine;

public class EnemyController : ZapController
{
    private const float INITIAL_X = 5.46f;

    protected override float getInitialX() {
        return INITIAL_X;
    }

    void FixedUpdate() {
    }

}

