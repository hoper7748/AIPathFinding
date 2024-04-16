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
        private Func<float, LayerMask, GameObject> callback;
        private LayerMask layerMask;
        public SearchNode(float _range, EnemyAI _ai, Func<float, LayerMask, GameObject> _callBack, string _name/*, LayerMask _layerMask*/)
        {
            range = _range;

            ai = _ai;
            callback = _callBack;
            name = _name;
            //layerMask = _layerMask;
        }

        public override NodeState Evaluate()
        {
            #region legacy
            //target = ai.NowTarget == null ? callback(range) : ai.NowTarget.gameObject;       // �̰� ��� ���� ������ �ϴ� ���� �ƹ��͵� ���ϴ°Űŵ�? �׷��ٸ� �ڵ� ����ȭ�� �غ���.
            target = callback(range, 1 << 4);
            if (target == null)
            {
                if (ai.NowTarget != null)
                    ai.movingPoint = System.Tuple.Create(ai.NowTarget.position, false);
                if (ai.lostTarget == false)
                {
                    ai.lostTarget = true;
                }
                //ai.NowTarget = null;
                return NodeState.Failure;
            }
            ai.lostTarget = false;
            ai.NowTarget = target.transform;
            ai.movingPoint = null;
            return target != null ? NodeState.Success : NodeState.Failure;
            #endregion
            # region TestCode
            //try
            //{
            //    ai.NowTarget = ai.NowTarget == null ? callback(range).transform : ai.NowTarget;
            //}
            //catch
            //{
            //    ai.NowTarget = null;
            //}
            //// Ÿ���� ���� ���
            //if (ai.NowTarget == null || Vector3.Distance(ai.transform.position, ai.NowTarget.position) > range)
            //{
            //    if (!ai.lostTarget)
            //    {
            //        ai.lostTarget = true;
            //    }
            //    return NodeState.Failure;
            //}
            ////else if ()
            ////{
            ////    // �Ÿ� ���� �����
            ////    return NodeState.Failure;
            ////}
            //// ���� ��ǥ ���̿� ������ ���� ������ return NodeStateFailure;
            //// �̰� �̻��� �͵� �����µ�?
            //if (CantSeeTarget())
            //    return NodeState.Failure;

            //ai.lostTarget = false;
            //ai.movingPoint = null;
            //return NodeState.Success;
        #endregion
        }

        private bool CantSeeTarget()
        {
            // ��ǥ�� �Ⱥ��δ�...
            RaycastHit hit;
            Ray ray = new Ray(ai.transform.position, ai.transform.position - ai.NowTarget.position);
            if (Physics.Raycast(ray, out hit, layerMask))
                return false;

            // ������ ���� ����.
            return true;
        }

    }
}
