using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class SearchNode : BT.Node
    {
        private float range;
        private Transform target;
        private Transform origin;
        public Func<float, GameObject> callback;

        public SearchNode(float _range, Transform _target, Transform _origin, Func<float, GameObject> _callBack, string _name)
        {
            range = _range;
            target = _target;
            origin = _origin;
            callback = _callBack;
            name = _name;
        }

        public override NodeState Evaluate()
        {

            if (target == null)
                return NodeState.Failure;
            GameObject A = callback(range);

            return A != null ? NodeState.Success : NodeState.Failure;
            
            //float distance = Vector3.Distance(target.position , origin.position);
            //return distance <= range ? NodeState.Success : NodeState.Failure;
        }
    }
}
