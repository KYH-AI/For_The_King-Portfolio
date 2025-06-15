using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class Waiting : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.battleUnit.IsProgress)
        {
            context.battleUnit.ActiveToken(ActiveTime.MyTurnStart);
            if (!Managers.Camera.DelayAttack&& context.battleUnit.IsProgress) 
            {
                return State.Success;
            }
            return State.Running;
        }
        else { return State.Running; }
    }
}
