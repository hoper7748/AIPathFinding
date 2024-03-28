using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class GoToCoverNode : BT.Node
    {

        private NavMeshAgent agent;
        private EnemyAI ai;

        public GoToCoverNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            Debug.Log($"{name}");
            Transform coverSpot = ai.GetBestCoverSpot();
            if (coverSpot == null)
                return NodeState.Failure;

            ai.SetColor(Color.blue);
            float distance = Vector3.Distance(coverSpot.position, agent.transform.position);

            if(distance > 0.2f)
            {
                agent.isStopped = false;
                agent.SetDestination(coverSpot.position);
                return NodeState.Running;
            }
            else
            {
                agent.isStopped = true;
                return NodeState.Success;
            }
        }
    }
}

