using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class IsCoveredNode : BT.Node
    {

        private Transform target;
        private Transform origin;

        public IsCoveredNode(Transform _target, Transform _origin)
        {
            target = _target;
            origin = _origin;
        }

        public override NodeState Evaluate()
        {
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