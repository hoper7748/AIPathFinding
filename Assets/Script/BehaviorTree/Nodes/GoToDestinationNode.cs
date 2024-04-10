using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BT
{
    public class GoToDestinationNode : BT.Node
    {
        EnemyAI ai;
        NavMeshAgent agent;

        public GoToDestinationNode(EnemyAI _ai, NavMeshAgent _agent)
        {
            ai = _ai;
            agent = _agent;
        }

        public override NodeState Evaluate()
        {

            // 둘 다 없을 경우.
            if (ai.NowTarget == null && ai.movingPoint == null) return NodeState.Failure;

            ai.SetColor(Color.yellow);
            Vector3 temp = ai.NowTarget == null ? ai.movingPoint.Item1 : ai.NowTarget.position;

            float distance = Vector3.Distance(temp, agent.transform.position);
            if (distance > 1.0f)
            {
                //Debug.Log($"navmesh Distance = {agent.remainingDistance }");
                //Debug.Log($"world Space Distance = {Vector3.Distance(ai.movingPoint.Item1, ai.transform.position).ToString()}");
                agent.isStopped = false;
                agent.SetDestination(temp);
                return NodeState.Running;
            }
            else
            {
                agent.isStopped = true;
                ai.movingPoint = null;
                return NodeState.Success;
            }
        }
    }

}