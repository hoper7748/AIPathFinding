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
            // 체력이 0 아래로 떨어지게 되면 사망.
            return ai.GetCurrentHealth() <= 0 ? NodeState.Success : NodeState.Failure;
        }
    }
}
