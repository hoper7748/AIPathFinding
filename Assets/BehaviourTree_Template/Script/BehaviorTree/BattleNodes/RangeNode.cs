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
        public LayerMask hideMask;
        public LayerMask targetMask;
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            GameObject target = agent.FindViewTarget(range,1 << 9, 1 << 3);
            if (target != null)
            {
                agent.target = target;

                return State.Success;
            }
            return State.Failure;
        }


        // -> �Ÿ��� üũ �� ������ ������.
        private bool RangeCheck()
        {
            agent.target = agent.FindViewTarget(range, 1 << 9, 1 << 3);
            if (agent.target == null)
                return false;
            return true;
        }
    }

}