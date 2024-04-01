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
            Vector3 temp = ai.movingPoint.Item1;

            float distance = Vector3.Distance(temp, agent.transform.position);
            if (distance > 1f)
            {
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