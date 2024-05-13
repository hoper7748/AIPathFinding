using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    // ��� ��� 
    // ���� - �߰��� �ƴ� �̵����� ��ǥ ��ġ�� �����Ͽ��°�?
    // ��� - Yes - �� �� ���
    //      - No  - ��� �̵�.
    // ���� ���� �ؾ��ϳ�?
    // �ֺ� ���. �̵��ϴ� ������ �������� �� ��� 
    // 
    public class BoundaryNode : BT.Node
    {
        EnemyAI ai;
        NavMeshAgent agent;
        public BoundaryNode(EnemyAI _ai ,NavMeshAgent _agent)
        {
            ai = _ai;
            agent = _agent; 
        }

        public override NodeState Evaluate()
        {
            //if (agent.angularSpeed < 1) ;

            if (true)
                return NodeState.Running;
            return NodeState.Failure;
        }
    }
}
