using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    // 숨었는지 확인하는 노드.
    // 숨어있는지 확인하고 다음 행동을 취함
    // 다음 행동 -> AmbushNode
    public class IsHideNode : BT.Node
    {
        EnemyAI ai;
        public IsHideNode(EnemyAI _ai)
        {
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            if (!ai.isHide)
            {
                return NodeState.Failure;
            }

            if (ai.GetCurrentHealth() > 50)
                ai.isHide = false;

            if(ai.GetCurrentHealth() >= 100)
            {
                ai.getCurrentHealth = 100;
                //ai.hide = false;

                return NodeState.Failure;
            }

            return NodeState.Success;
        }
    }
}
