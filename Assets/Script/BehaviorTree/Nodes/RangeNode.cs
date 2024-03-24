using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class RangeNode : BT.Node
    {
        private float range;
        private Transform target;
        private Transform origin;

        public RangeNode(float _range, Transform _target, Transform _origin)
        {
            range = _range;
            target = _target;
            origin = _origin;
        }

        public override NodeState Evaluate()
        {
            float distance = Vector3.Distance(target.position , origin.position);
            return distance <= range ? NodeState.Success : NodeState.Failure;
        }
    }
}
