using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    //[NodeInfo]
    // 현재 숨을 수 있는 공간에 이동을 했는가?
    
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
                return State.Success;
            }
            return State.Running;
        }
    }

}