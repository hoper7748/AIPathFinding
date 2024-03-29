using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{

    // 움직일 위치를 찾았는지 체크하는 노드 ?
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
