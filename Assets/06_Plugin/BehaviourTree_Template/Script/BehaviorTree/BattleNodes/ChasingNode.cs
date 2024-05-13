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
        private static float curTimer;
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
            // ��... ������ �߰��� �ؾ��ϴϱ�... 
            if (CheckDistance() < 0.5f)
            {
                return State.Success;
            }

            // ã�Ҵٸ� State.Success�� ��ȯ
            GameObject target = agent.FindViewTarget(30f, 75f, 1 << 9, 1 << 3);
            if(target == null)
            {
                //Debug.Log("BB");
                return State.Running;
            }
            //Debug.Log("AA");
            Debug.Log($"{target.transform.name}");
            curTimer = 0;
            return State.Success;
        }

        private float CheckDistance()
        {
            try
            {
                float distance = Vector3.Distance(agent.getPathPosition, agent.getLastPath);
                if (distance == 0)
                    throw new System.Exception(distance.ToString());
                return distance;
            }
            catch
            {
                Debug.Log("Can't Find Path.\ni Waiting new Path.");
                return 99;
            }
        }
    }
}
