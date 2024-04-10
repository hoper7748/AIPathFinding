using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class IsAliveNode : BT.Node
    {
        EnemyAI ai;
        public IsAliveNode(EnemyAI _ai)
        {
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            // ü���� 0 �Ʒ��� �������� �Ǹ� ���.
            return ai.GetCurrentHealth() <= 0 ? NodeState.Success : NodeState.Failure;
        }
    }
}
