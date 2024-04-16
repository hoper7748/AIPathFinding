using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    // 본 노드는 매복 노드.
    // 이거 Search와 하는 효과는 같음.
    // 안써도 될 듯? 지울 예정
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
            // 현재 숨어있는 상태

            // 타겟이 없으면 타겟을 찾아주고 있으면 그대로 가져간다. -> 연산 최소화를 위해 사망연산자를 사용 
            target = target == null ? ai.FindViewTarget(30f, 1 << 4).transform : target;    
            if(target == null)
            {
                // 없다는 뜻은 아직 찾고있다는 뜻.
                Debug.Log("Enemy Searching");
                return NodeState.Running;
            }
            else
            {
                // 적을 발견하면 적을 향해 발사하기 위해 Success를 넘겨줌.

                Debug.Log("Ambush Node");

                return NodeState.Success;
            }

        }
    }
}
