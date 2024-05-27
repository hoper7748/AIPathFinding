using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{

    public class LookAtNode : ActionNode
    {
        Vector3 dir;
        float distance = 0;
        protected override void OnStart()
        {
            // lookPosition�� �ٶ��� 
            dir = agent.LookPosition - agent.transform.position;
            distance = Vector3.Distance(agent.transform.position, agent.LookPosition);
            //Debug.Log($"{distance}");
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // ���� ����ġ ���� ������ ����� ���ο� �۾��� �Ϸ� ��.
            if (false)
                return State.Failure;

            // �ٶ󺸰� �� ���ε�... �ôٴ� ���� üũ�Ϸ��� Ray�� ���߰ڴ�.
            if(ForwradCheck())
            {
                return State.Success;
            }
            return State.Running;   
        }

        // ray�� ���� üũ �� ��
        private bool ForwradCheck()
        {
            // Ray�� ���� ���� ���ֺ� �� ���� üũ
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation(dir), 3 * Time.deltaTime);
            if (Physics.Raycast(agent.transform.position, agent.transform.forward, distance * 1.5f, 1<< 9 ))
            {
                return true;
            }
            return false;
        }

    }
}