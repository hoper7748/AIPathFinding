using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// 상대가 보일 때 까지 추적. 그러나, 일정 시간동안 이동했음에도 찾을 수 없다면 State.Failure을 반환.
    /// </summary>
    public class ChasingNode : ActionNode
    {
        public float limitedSearchTime;
        public float searchAngle;
        float curTimer;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // 임시로 추적하기
            curTimer += Time.deltaTime;
            if(curTimer > limitedSearchTime)
            {
                agent.isEncounter = false;
                agent.target = null;
                curTimer = 0;
                // 시간내에 찾지 못한다면 State.Failre을 반환
                return State.Failure;
            }

            // 찾았다면 State.Success를 반환
            GameObject target = agent.FindViewTarget(30f, 75f, 1 << 9, 1 << 3);
            if(target != null)
            {
                return State.Success;
            }


            return State.Success;
        }
    }
}
