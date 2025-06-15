using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

[System.Serializable]
public class SelectPlayer : ActionNode
{
    private List<BattleUnit> targets = new List<BattleUnit>();
    Vector3 _vectorAB;
    Vector3 _nowpos;
    float _angle;
    protected override void OnStart()
    {
        targets = Managers.Battle.PlayerList;
        if (targets.Count != 0)
        {
            int rand = Random.Range(0, targets.Count);
            context.battleUnit.Target = targets.ElementAt(rand);

        }
        _nowpos = context.gameObject.GetComponent<BattleUnit>().nowpos;
        targets = Managers.Battle.PlayerList;
        _vectorAB = context.battleUnit.Target.nowpos - _nowpos;
        _angle = Mathf.Atan2(_vectorAB.z, _vectorAB.x) * Mathf.Rad2Deg;
        context.gameObject.transform.rotation = Quaternion.Euler(0, 90 - _angle, 0f);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
       
        return State.Success;
    }
}
