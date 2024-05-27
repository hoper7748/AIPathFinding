using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class IsCovverAndConcealment : ActionNode
    {
        // 마지막으로 확인 된 방향에 뭔가 없을 때 까지 이동
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {
            // ray를 쏴서 해당 방향에 벽이 존재하는지 체크 
            if (IsWallCheck())
            {
                // 이동 실시
                return State.Running;
            }
            return State.Success;
        }

        private void Movement()
        {
;

        }

        private bool IsWallCheck()
        {
            RaycastHit hit;
            Ray ray = new Ray(agent.transform.position, agent.transform.position - agent.LostTargetPoint );
            // 벽이 바라봐야할 방향을 가리고 있지 않다면 현재 바라보는 방향으로 공격한다.
            if(Physics.Raycast(ray, out hit, 2f, 1 << 9))
            {
                return true;
            }

            return false;
        }
    }
}