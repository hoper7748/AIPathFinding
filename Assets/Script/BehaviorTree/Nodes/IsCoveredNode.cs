using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class IsCoveredNode : BT.Node
    {

        //private Transform target;
        private EnemyAI ai;

        public IsCoveredNode(Transform _target, EnemyAI _ai, string _name)
        {
            //target = _target;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {

            RaycastHit hit;
            if(ai.NowTarget == null)
            {
                return NodeState.Failure;
            }


            // Target이 없으면?
            //Debug.DrawLine(origin.transform.position, origin.transform.position + (origin.NowTarget.position - origin.transform.position).normalized * Vector3.Distance(origin.NowTarget.position, origin.transform.position), Color.blue);
            if (Physics.Raycast(ai.transform.position, ai.NowTarget.position - ai.transform.position, out hit))
            {
                if (hit.collider.transform != ai.NowTarget)
                {
                    ai.NowTarget = null;
                    ai.isHide = true;
                    ai.isRun = false;
                    ai.movingPoint = null;
                    return NodeState.Success;
                }
            }
            return NodeState.Failure;
        }
    }

}