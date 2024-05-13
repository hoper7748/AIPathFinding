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
        private static float curTimer;
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
            // 흠... 역으로 추격을 해야하니까... 
            if (CheckDistance() < 0.5f)
            {
                return State.Success;
            }

            // 찾았다면 State.Success를 반환
            GameObject target = agent.FindViewTarget(30f, 75f, 1 << 9, 1 << 3);
            if(target == null)
            {
                //Debug.Log("BB");
                return State.Running;
            }
            //Debug.Log("AA");
            Debug.Log($"{target.transform.name}");
            curTimer = 0;
            return State.Success;
        }

        private float CheckDistance()
        {
            try
            {
                float distance = Vector3.Distance(agent.getPathPosition, agent.getLastPath);
                if (distance == 0)
                    throw new System.Exception(distance.ToString());
                return distance;
            }
            catch
            {
                Debug.Log("Can't Find Path.\ni Waiting new Path.");
                return 99;
            }
        }
    }
}
