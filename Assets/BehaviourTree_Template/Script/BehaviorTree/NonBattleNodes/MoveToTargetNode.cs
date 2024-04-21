using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    /// <summary>
    /// ��ǥ �������� �̵���Ű�� ���.
    /// </summary>
    public class MoveToTargetNode : ActionNode
    {
        protected override void OnStart()
        {
            //agent.navMeshAgent.isStopped = false;
            agent.StopPathFinding();
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            //��ǥ �������� �̵�.
            //if (agent.target != null)
            //    return State.Failure;

            // ������ �ּ�ȭ �ϱ� ���� ��ȯ ������ ������ ������ �޸���.
            // ��ǥ�� ���� ��밡 �ִµ� ���� ���� ������ �� �ʿ�� ����.
            if (agent.target != null)
            {
                pathFinding.PathRequestManager.RequestPath(agent.transform.position, agent.target.transform.position, agent.OnPathFound);
                //agent.navMeshAgent.SetDestination(agent.target.transform.position);
                return State.Success;
            }
            else if (SetDestination())
            {
                return State.Success;
            }

            return State.Failure;
            //return State.Success;

        }

        //private void MoveTarget()
        //{
        //    // ��ǥ ���� ����
        //    agent.navMeshAgent.SetDestination(blackboard.moveToObject == null ? blackboard.moveToPosition : blackboard.moveToObject.position);
        //}

        private bool SetDestination()
        {
            NavMeshHit hit;
            for (int i = 0; i < 30; i++)
            {
                Vector3 dir = Random.insideUnitSphere;
                //Vector3 dir2 = 
                Vector3 randomPoint = agent.transform.position +  dir * 10f;
                //if ( Vector3.Distance(randomPoint, agent.transform.position) >= 3f && NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                if (Vector3.Distance(randomPoint, agent.transform.position) >= 3f && pathFinding.PathRequestManager.IsMovementPoint(randomPoint))
                {
                    //agent.navMeshAgent.SetDestination(hit.position);
                    // ��ǥ ������ ���� �̵�!
                    pathFinding.PathRequestManager.RequestPath(agent.transform.position, randomPoint, agent.OnPathFound);
                    return true;
                }
            }
            return false;
        }
    }
}
