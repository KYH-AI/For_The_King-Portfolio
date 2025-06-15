using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

[System.Serializable]
public class SelectRandomE : ActionNode
{
    public void PopUpInit()
    {
        List<Skill> _skillList = context.PlayerBattle.SkillList;
        context.uI_BattlePopUp.BtnEvent(_skillList);
        context.uI_BattlePopUp.Init();
        context.uI_BattlePopUp.SetSkills(_skillList);
        context.uI_BattlePopUp.ChangeText(context.PlayerBattle.SkillList[0]);
    }

    protected override void OnStart()
    {
        PopUpInit();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (Managers.Battle.EnemyList.Count != 0)
        {
            int rand = Random.Range(0, Managers.Battle.EnemyList.Count);
            context.battleUnit.Target = Managers.Battle.EnemyList[rand];
        }
        


        return State.Success;
    }
}
