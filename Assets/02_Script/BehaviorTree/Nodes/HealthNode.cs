using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    public class HealthNode : BT.Node
    {
        private EnemyAI ai;
        private float threshold;

        public HealthNode(EnemyAI _ai, float _threshold, string _name)
        {
            ai = _ai;
            threshold = _threshold;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log($"{name}");

            return ai.GetCurrentHealth() <= threshold ? NodeState.Success : NodeState.Failure;
        }
    }

}