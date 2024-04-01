using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class ChaseNode : BT.Node
    {
        private NavMeshAgent agent;
        private EnemyAI ai;
        private Transform hidingObject;
        private System.Tuple<Vector3> v;

        // 이동 중 가까운 거리(1 ~ 2?) 내에 벽이 체크되면 벽에 붙어서 이동.
        // 벽으로 이동하기 전, 마지막 위치를 기억.
        // 벽으로 이동 후 Hide상태가 체크되며 마지막으로 기억한 위치로 재 탐색.
        // Ray에 체크되는 것이 없으면 체크되지 않는 방향으로 경계 후 기존의 이동하려는 위치로 이동함.

        public ChaseNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            // 둘 다 없을 경우.
            if (ai.NowTarget == null && ai.movingPoint == null ) return NodeState.Failure;

            ai.SetColor(Color.yellow);
            Vector3 temp = ai.NowTarget == null ? ai.movingPoint.Item1 : ai.NowTarget.position;
            float distance = Vector3.Distance(temp, agent.transform.position);
            if(distance > 1f)
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

