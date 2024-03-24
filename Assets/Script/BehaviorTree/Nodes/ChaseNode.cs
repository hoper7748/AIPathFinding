using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class ChaseNode : BT.Node
    {
        private Transform target;
        private NavMeshAgent agent;
        private EnemyAI ai;

        public ChaseNode(Transform _target, NavMeshAgent _agent, EnemyAI _ai)
        {
            target = _target;
            agent = _agent;
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            ai.SetColor(Color.yellow);
            float distance = Vector3.Distance(target.position, agent.transform.position);
            if(distance > 0.2f)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
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

