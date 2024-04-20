using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    /// <summary>
    /// 목표 지점에 도달했는지 체크하는 노드 
    /// Running과 Success로 이동과 도착을 표시하고
    /// Failure로 사건사고를 체크한다.
    /// 
    /// </summary>
    public class ArriveDestinationNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (CheckDistance() >= 1.1f)
            {
                return State.Running;
            }

            if (agent.target != null && !agent.isHit)
            {
                // 이동을 종료하고 다음 행동을 진행함.
                agent.navMeshAgent.isStopped = true;
                return State.Failure;
            }

            return State.Success;
        }

        private float CheckDistance()
        {
            //float distance = Vector3.Distance(agent.transform.position, blackboard.moveToObject == null ? blackboard.moveToPosition : blackboard.moveToObject.position);
            float distance = Vector3.Distance(agent.transform.position, agent.navMeshAgent.destination);
            return distance;
        }
    }

}