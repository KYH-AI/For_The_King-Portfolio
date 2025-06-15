using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class TurnOver : ActionNode
{
    protected override void OnStart()
    {
        context.battleUnit.CountDownToken(ActiveTime.Attack);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {

        context.battleUnit.IsProgress = false;
        if (Managers.Battle.EnemyList.Count == 0)
        {
            Managers.Battle.Ending(false);
        }
        else
        {
            Managers.Battle.TurnOver();
        }
        int alpha =0;
        if (Managers.Battle.IsCave)
        {
            alpha = 180;
        }
        if (context.gameObject.CompareTag("Enemy"))
        {
            context.gameObject.transform.rotation = Quaternion.Euler(0, alpha + 90, 0f);
        }
        else
        {
            context.gameObject.transform.rotation = Quaternion.Euler(0, alpha - 90, 0f);
        }
        

        return State.Success;
    }
}
