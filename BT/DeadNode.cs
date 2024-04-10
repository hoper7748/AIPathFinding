using System.Collections;
using UnityEngine;

namespace BT
{
    class DeadNode : BT.Node
    {
        private EnemyAI ai;

        public DeadNode(EnemyAI _ai)
        {
            this.ai = _ai;
        }

        public override NodeState Evaluate()
        {
            ai.DeadPlayer();
            GameObject.Destroy(ai.gameObject, 2f);
            ai.tag = "Dead";

            return NodeState.Success;
        }
    }
}