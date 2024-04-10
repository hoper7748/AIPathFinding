using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    // �� ���� �ź� ���.
    // �̰� Search�� �ϴ� ȿ���� ����.
    // �Ƚᵵ �� ��? ���� ����
    public class AmbushNode : BT.Node
    {
        EnemyAI ai;
        Transform target;
        public AmbushNode(EnemyAI _ai)
        {
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            // ���� �����ִ� ����

            // Ÿ���� ������ Ÿ���� ã���ְ� ������ �״�� ��������. -> ���� �ּ�ȭ�� ���� ��������ڸ� ��� 
            target = target == null ? ai.FindViewTarget(30f, 1 << 4).transform : target;    
            if(target == null)
            {
                // ���ٴ� ���� ���� ã���ִٴ� ��.
                Debug.Log("Enemy Searching");
                return NodeState.Running;
            }
            else
            {
                // ���� �߰��ϸ� ���� ���� �߻��ϱ� ���� Success�� �Ѱ���.

                Debug.Log("Ambush Node");

                return NodeState.Success;
            }

        }
    }
}
