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
            agent.StopPathFinding();
        }

        protected override State OnUpdate()
        {
            RaycastHit hit;

            Vector3 back = agent.transform.position - agent.transform.forward * 0.5f;
            if (Physics.Raycast(back, agent.target.transform.position - back, out hit, 5f, 1 << 9))
            {
                agent.isHit = false;
                agent.isHide = true;
                agent.SetBestCoverSopt(null);
                agent.LookPosition = hit.point;
                // ���� ��ġ�� �Ҿ���.
                agent.isLostTarget = true;
                // ���� ����
                agent.LostTargetPoint = agent.target.transform.position;
                return State.Success;

            }
            return State.Running;
        }
    }

}