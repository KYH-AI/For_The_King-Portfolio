using TheKiwiCoder;

[System.Serializable]
public class CalDamage : ActionNode
{

    protected override void OnStart() {
        if (context.transform.CompareTag("Player")){
            context.uI_BattlePopUp.SetRandomSlot(context.battleUnit);
        }
        else
        {
            context.uI_EnemySlot.SetRandomEnemySlot(context.battleUnit, context.battleUnit.OnSkill);
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.battleUnit.IsRun)
        {
            context.battleUnit.IsRun = false;
            return State.Failure;
        }
        if (!context.battleUnit.IsAttack)
        {
            return State.Running;
            
        }
        else
        {
            context.battleUnit.IsAttack = false;
            return State.Success;
        }
    }   
}
