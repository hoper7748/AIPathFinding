using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class IsCoveredNode : BT.Node
    {

        //private Transform target;
        private EnemyAI origin;

        public IsCoveredNode(Transform _target, EnemyAI _origin, string _name)
        {
            //target = _target;
            origin = _origin;
            name = _name;
        }

        public override NodeState Evaluate()
        {

            RaycastHit hit;

            //Debug.DrawLine(origin.transform.position, origin.transform.position + (origin.NowTarget.position - origin.transform.position).normalized * Vector3.Distance(origin.NowTarget.position, origin.transform.position), Color.blue);
            if (Physics.Raycast(origin.transform.position, origin.NowTarget.position - origin.transform.position, out hit))
            {
                if (hit.collider.transform != origin.NowTarget)
                {
                    return NodeState.Success;
                }
            }
            return NodeState.Failure;
        }
    }

}