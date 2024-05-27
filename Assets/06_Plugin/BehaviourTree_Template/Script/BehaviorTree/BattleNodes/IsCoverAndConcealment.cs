using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class IsCoverAndConcealment : ActionNode
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
            return State.Success;
        }

        private void Movement()
        {
;

        }

    }
}