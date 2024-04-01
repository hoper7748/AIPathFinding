using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class SearchNode : BT.Node
    {
        private float range;
        private GameObject target;
        private EnemyAI ai;
        public Func<float, GameObject> callback;

        public SearchNode(float _range, EnemyAI _ai, Func<float, GameObject> _callBack, string _name)
        {
            range = _range;
            //target = _target.gameObject;
            ai = _ai;
            callback = _callBack;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            target = callback(range);
            if (target == null )
            {
                if(ai.NowTarget != null)
                    ai.movingPoint = System.Tuple.Create(ai.NowTarget.position, false);
                ai.NowTarget = null;
                return NodeState.Failure;
            }

            ai.NowTarget = target.transform;
            ai.movingPoint = null;
            return target != null ? NodeState.Success : NodeState.Failure;
        }
    }
}
