using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{

    // ������ ��ġ�� ã�Ҵ��� üũ�ϴ� ��� ?
    public class HasTargetNode : BT.Node
    {
        EnemyAI ai;
        public HasTargetNode(EnemyAI _ai)
        {
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log("TestNode");
            return NodeState.Success;
        }
    }
}
