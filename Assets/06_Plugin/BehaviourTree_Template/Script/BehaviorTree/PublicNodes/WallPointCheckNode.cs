using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class WallPointCheckNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            Vector3 wallBackPoint = agent.FindViewTargetHitPoint(30, 360, 1 << 9, 1 << 3);            
            
            pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(agent.transform.position, wallBackPoint, agent.OnPathFound));
            return State.Success;

        }
    }
}