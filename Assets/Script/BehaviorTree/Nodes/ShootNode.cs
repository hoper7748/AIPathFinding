using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class ShootNode : BT.Node
    {
        private NavMeshAgent agent;
        private EnemyAI ai;

        public ShootNode(NavMeshAgent _agent, EnemyAI _ai)
        {
            agent = _agent;
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            agent.isStopped = true;
            ai.SetColor(Color.green);
            return NodeState.Success;
        }
    }
}
