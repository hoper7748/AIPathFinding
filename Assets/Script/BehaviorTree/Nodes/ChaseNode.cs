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

        // �̵� �� ����� �Ÿ�(1 ~ 2?) ���� ���� üũ�Ǹ� ���� �پ �̵�.
        // ������ �̵��ϱ� ��, ������ ��ġ�� ���.
        // ������ �̵� �� Hide���°� üũ�Ǹ� ���������� ����� ��ġ�� �� Ž��.
        // Ray�� üũ�Ǵ� ���� ������ üũ���� �ʴ� �������� ��� �� ������ �̵��Ϸ��� ��ġ�� �̵���.

        public ChaseNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            // �� �� ���� ���.
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

