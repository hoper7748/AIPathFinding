using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    public class IsRunNode : Node
    {
        EnemyAI ai;
        public IsRunNode(EnemyAI _ai, string _name)
        {
            ai = _ai;
            name = _name;
        }
        
        public override NodeState Evaluate()
        {
            return ai.isRun ? NodeState.Success : NodeState.Failure;
        }
    }

}