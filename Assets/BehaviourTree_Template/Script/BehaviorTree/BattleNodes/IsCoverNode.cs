using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    //[NodeInfo]
    // ���� ���� �� �ִ� ������ �̵��� �ߴ°�?
    
    public class IsCoverNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            //agent.navMeshAgent.isStopped = true;
            agent.StopPathFinding();
        }

        protected override State OnUpdate()
        {
            //if (agent.target == null)
            //{
            //    return State.Failure;
            //}

            RaycastHit hit;

            // Debug.DrawLine(origin.transform.position, origin.transform.position + (origin.NowTarget.position - origin.transform.position).normalized * Vector3.Distance(origin.NowTarget.position, origin.transform.position), Color.blue);
            // ���� �ٶ󺸴� �������� 2m �Ÿ��� ���� �ִ°�
            Vector3 back = agent.transform.position - agent.transform.forward * 0.5f;
            if (Physics.Raycast(back, agent.target.transform.position - back, out hit, 5f, 1 << 9))
            {
                // ���� target�� �ٶ󺸴� ���� target�� �ƴ� ��ü�� �����ϴ°�? �����ϸ� 
                // �� �ڿ� �����°�?
                agent.isHit = false;
                agent.isHide = true;
                agent.SetBestCoverSopt(null);
                agent.LookPosition = hit.point;
                //agent.transform.LookAt(hit.point); // <- ����� �ٶ󺸰� �ؾ� �� . ��� �ٶ󺸰� �� ���ΰ�?
                // ������! �׷� ������ �ؾ��ұ�?
                return State.Success;
            }
            // ���� ���� �ʾҾ�� -> ���� ���� ã�ƾ��ؿ�
            return State.Running;
        }
    }

}