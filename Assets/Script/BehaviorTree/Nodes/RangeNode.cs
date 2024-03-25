using System;
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
        public Func<Transform, float, LayerMask, GameObject> callback;
        public LayerMask layerMask;

        public RangeNode(float _range, Transform _target, Transform _origin, Func<Transform, float, LayerMask, GameObject> _callBack, LayerMask _layerMask,string _name)
        {
            range = _range;
            target = _target;
            origin = _origin;
            callback = _callBack;
            layerMask = _layerMask;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            
            float distance = Vector3.Distance(target.position , origin.position);
            GameObject A = callback(origin.transform, range, layerMask);
            

            //return distance <= range ? NodeState.Success : NodeState.Failure;
            return A != null ? NodeState.Success : NodeState.Failure;

        }
    }
}
