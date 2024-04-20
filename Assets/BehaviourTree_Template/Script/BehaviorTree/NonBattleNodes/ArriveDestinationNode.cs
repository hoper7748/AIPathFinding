using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    /// <summary>
    /// ��ǥ ������ �����ߴ��� üũ�ϴ� ��� 
    /// Running�� Success�� �̵��� ������ ǥ���ϰ�
    /// Failure�� ��ǻ�� üũ�Ѵ�.
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
                // �̵��� �����ϰ� ���� �ൿ�� ������.
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