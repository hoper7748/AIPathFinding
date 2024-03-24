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

        public RangeNode(float _range, Transform _target, Transform _origin, string _name)
        {
            range = _range;
            target = _target;
            origin = _origin;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            Debug.Log($"{name}");
            float distance = Vector3.Distance(target.position , origin.position);
            if (distance <= range)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Failure");

            }
            return distance <= range ? NodeState.Success : NodeState.Failure;
        }
    }
}
