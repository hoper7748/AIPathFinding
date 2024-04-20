using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    // 다음 State를 일정 시간동안 대기하는 노드.
    internal class WaitNode : ActionNode
    {
        public float duration = 1;
        float startTime;

        protected override void OnStart()
        {
            startTime = Time.time;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if(Time.time - startTime > duration)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}
