using TheKiwiCoder;

[System.Serializable]
public class Attack : ActionNode
{

    protected override void OnStart()
    {
        context.battleUnit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Skill1);
       
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
