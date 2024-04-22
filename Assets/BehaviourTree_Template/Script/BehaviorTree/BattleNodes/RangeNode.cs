using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// �Ÿ� ���ο� ���� ������ Ȯ���ϴ� ���
    /// </summary>

    public class RangeNode : ActionNode
    {
        // �ʿ��� �Ÿ�.
        public float range;
        [Space(10f)]
        public float searchAngler;
        public LayerMask hideMask;
        public LayerMask targetMask;
        
        protected override void OnStart()
        {
            description = "Hello";
        }

        protected override void OnStop()
        {
            description = "";
        }

        protected override State OnUpdate()
        {
            GameObject target = agent.FindViewTarget(range, searchAngler, 1 << 9, 1 << 3);
            
            if (target != null)
            {
                agent.target = target;
                agent.TargetPosition = target.transform.position;
                return State.Success;
            }
            return State.Failure;
        }


        // -> �Ÿ��� üũ �� ������ ������.
        private bool RangeCheck()
        {
            agent.target = agent.FindViewTarget(range, searchAngler, 1 << 9, 1 << 3);
            if (agent.target == null)
                return false;
            return true;
        }
    }

}