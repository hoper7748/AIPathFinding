using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// �ǰ��� ���� �������� üũ�ϴ� ���
    /// </summary>
    public class IsHitNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // �ǰ��� ���� �����ΰ�?
            if (agent.isHit)
            {
                agent.isEncounter = true;

                return State.Success;
            }
            return State.Failure;
        }
    }
}
