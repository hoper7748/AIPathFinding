using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class DeadNode : BT.Node
    {
        EnemyAI ai;
        public DeadNode(EnemyAI _ai)
        {
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            ai.DeadPlayer();
            GameObject.Destroy(ai.gameObject, 1f);
            //ai.GetComponent<EnemyAI>().enabled = false;
            ai.tag = "Dead";

            return NodeState.Success;
        }
    }

}
