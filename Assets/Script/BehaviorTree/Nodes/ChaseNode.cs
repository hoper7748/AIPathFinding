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

        public ChaseNode(Transform _target, NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            target = _target;
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            if (target == null) return NodeState.Failure;

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

