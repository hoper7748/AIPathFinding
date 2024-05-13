using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// 피격을 당한 상태인지 체크하는 노드
    /// </summary>
    public class IsHitNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // 피격을 당한 상태인가?
            if (agent.isHit)
            {
                agent.isEncounter = true;

                return State.Success;
            }
            return State.Failure;
        }
    }
}
