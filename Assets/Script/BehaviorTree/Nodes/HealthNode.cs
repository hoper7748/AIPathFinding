using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    public class HealthNode : BT.Node
    {
        private EnemyAI ai;
        private float threshold;                // ´ÄÀº?

        public HealthNode(EnemyAI _ai, float _threshold)
        {
            ai = _ai;
            threshold = _threshold;
        }

        public override NodeState Evaluate()
        {

            return ai.GetCurrentHealth() <= threshold ? NodeState.Success : NodeState.Failure;
        }
    }

}