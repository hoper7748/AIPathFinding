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

        public ShootNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            Debug.Log($"{name}");
            agent.isStopped = true;
            ai.SetColor(Color.green);
            return NodeState.Success;
        }
    }
}
