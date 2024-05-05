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
        float a;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            Debug.Log($"Last Check Distance {a}");
        }

        float curTiemr = 0;
        float delayTimer = 0;

        protected override State OnUpdate()
        {
            a = CheckDistance();
            if (a > 0.5)
            {
                //Debug.Log($"{a}");
                return State.Running;
            }

            if (agent.target != null && !agent.isHit)
            {
                // �̵��� �����ϰ� ���� �ൿ�� ������.
                //agent.navMeshAgent.isStopped = true;
                agent.StopPathFinding();
                return State.Failure;
            }

            return State.Success;
        }

        private float CheckDistance()
        {
            //float distance = Vector3.Distance(agent.transform.position, blackboard.moveToObject == null ? blackboard.moveToPosition : blackboard.moveToObject.position);
            //float distance = Vector3.Distance(agent.transform.position, agent.navMeshAgent.destination);
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