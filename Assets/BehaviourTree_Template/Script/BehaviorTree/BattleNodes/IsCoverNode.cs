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
            // 적을 바라보는 방향으로 2m 거리에 벽이 있는가
            Vector3 back = agent.transform.position - agent.transform.forward * 0.5f;
            if (Physics.Raycast(back, agent.target.transform.position - back, out hit, 5f, 1 << 9))
            {
                // 현재 target을 바라보는 곳에 target이 아닌 물체가 존재하는가? 존재하면 
                // 벽 뒤에 숨었는가?
                agent.isHit = false;
                agent.isHide = true;
                agent.SetBestCoverSopt(null);
                agent.LookPosition = hit.point;
                //agent.transform.LookAt(hit.point); // <- 여기로 바라보게 해야 함 . 어떻게 바라보게 할 것인가?
                // 숨었다! 그럼 무엇을 해야할까?
                return State.Success;
            }
            // 벽에 숨지 않았어요 -> 숨을 곳을 찾아야해요
            return State.Running;
        }
    }

}