using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// 거리 내부에 적이 들어온지 확인하는 노드
    /// </summary>

    public class RangeNode : ActionNode
    {
        // 필요한 거리.
        public float range;
        public LayerMask hideMask;
        public LayerMask targetMask;
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            GameObject target = agent.FindViewTarget(range,1 << 9, 1 << 3);
            if (target != null)
            {
                agent.target = target;

                return State.Success;
            }
            return State.Failure;
        }


        // -> 거리를 체크 후 공격을 진행함.
        private bool RangeCheck()
        {
            agent.target = agent.FindViewTarget(range, 1 << 9, 1 << 3);
            if (agent.target == null)
                return false;
            return true;
        }
    }

}