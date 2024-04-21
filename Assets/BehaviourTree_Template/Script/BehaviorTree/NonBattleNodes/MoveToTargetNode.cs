using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    /// <summary>
    /// 목표 지점까지 이동시키는 노드.
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
            //목표 지점으로 이동.
            //if (agent.target != null)
            //    return State.Failure;

            // 연산을 최소화 하기 위해 반환 내용은 같으나 조건을 달리함.
            // 목표로 삼을 상대가 있는데 굳이 새로 세팅해 줄 필요는 없음.
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
        //    // 목표 지점 설정
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
                    // 목표 지점을 향해 이동!
                    pathFinding.PathRequestManager.RequestPath(agent.transform.position, randomPoint, agent.OnPathFound);
                    return true;
                }
            }
            return false;
        }
    }
}
