using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class IsCoveredNode : BT.Node
    {

        private Transform target;
        private Transform origin;

        public IsCoveredNode(Transform _target, Transform _origin, string _name)
        {
            target = _target;
            origin = _origin;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log($"{name}");
            RaycastHit hit;
            if (Physics.Raycast(origin.position, target.position - origin.position, out hit))
            {
                if (hit.collider.transform != target)
                {
                    return NodeState.Success;
                }
            }
            return NodeState.Failure;
        }
    }

}