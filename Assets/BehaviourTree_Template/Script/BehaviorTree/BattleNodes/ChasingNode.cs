using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// ��밡 ���� �� ���� ����. �׷���, ���� �ð����� �̵��������� ã�� �� ���ٸ� State.Failure�� ��ȯ.
    /// </summary>
    public class ChasingNode : ActionNode
    {
        public float limitedSearchTime;
        public float searchAngle;
        float curTimer;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // �ӽ÷� �����ϱ�
            curTimer += Time.deltaTime;
            if(curTimer > limitedSearchTime)
            {
                agent.isEncounter = false;
                agent.target = null;
                curTimer = 0;
                // �ð����� ã�� ���Ѵٸ� State.Failre�� ��ȯ
                return State.Failure;
            }

            // ã�Ҵٸ� State.Success�� ��ȯ
            GameObject target = agent.FindViewTarget(30f, 75f, 1 << 9, 1 << 3);
            if(target != null)
            {
                return State.Success;
            }


            return State.Success;
        }
    }
}
