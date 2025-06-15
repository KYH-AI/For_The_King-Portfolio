using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MoveToPos : ActionNode
{
    float speed = 7;
    protected override void OnStart()
    {
 
       
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        context.gameObject.transform.localPosition = Vector3.MoveTowards
            (context.gameObject.transform.localPosition, blackboard.moveToPosition, speed * Time.deltaTime);
        


        if (context.gameObject.transform.localPosition != blackboard.moveToPosition)
        {
            return State.Running;
        }
        else
        {
            return State.Success;
        }
    }
}
