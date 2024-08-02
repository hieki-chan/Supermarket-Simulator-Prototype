using UnityEngine;

namespace Hieki.AI
{
    public class Wait : Node
    {
        readonly float delay;
        float delayTimer = .0f;

        public Wait(float time)
        {
            delay = time;
        }

        public override NodeState Evaluate()
        {
            delayTimer += Time.deltaTime;
            if( delayTimer >= delay)
            {
                delayTimer = 0;
                return currentState = NodeState.Success;
            }
            return currentState = NodeState.Running;
        }
    }
}
