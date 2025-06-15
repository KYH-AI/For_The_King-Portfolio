using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

[System.Serializable]
public class SelectRandomSkill : ActionNode
{
    Vector3 _nowpos;
    Vector3 _vectorAB;
    float _angle;
    bool _isTartgetChange = false;
    void SetHex()
    {
        if (context.battleUnit.OnSkill.Target == 1)
        {
            context.battleUnit.FXList[1].SetActive(true);
            context.battleUnit.FXList[0].gameObject.SetActive(false);
            context.battleUnit.FXList[1].transform.localEulerAngles = new Vector3(-90, 180 - _angle, 0);
        }
        else
        {
            context.battleUnit.FXList[0].gameObject.SetActive(true);
            context.battleUnit.FXList[1].SetActive(false);
            context.battleUnit.FXList[0].transform.localEulerAngles = new Vector3(-90, 180 - _angle, 0);

        }

    }
    protected override void OnStart() {
        _vectorAB = context.battleUnit.Target.nowpos - _nowpos;
        _angle = Mathf.Atan2(_vectorAB.z, _vectorAB.x) * Mathf.Rad2Deg;
        int rand = Random.Range(0, context.EnemyBattle.SkillList.Count);
        context.battleUnit.OnSkill = context.EnemyBattle.SkillList.ElementAt(rand);
        context.uI_EnemySlot.Init();
        context.uI_EnemySlot.SetSlot(context.battleUnit.OnSkill.Rolls, context.battleUnit.OnSkill.Type);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        
        return State.Success;
    }
}
