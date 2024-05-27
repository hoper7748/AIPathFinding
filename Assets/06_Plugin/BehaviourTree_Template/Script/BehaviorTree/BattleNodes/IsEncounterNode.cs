using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class IsEncounterNode : ActionNode
    {
        protected override void OnStart()
        {
            //agent.isEncounter =  ? true : false;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if(agent.isEncounter)
            {
                // 상대방 위치를 잃었다.
                agent.isLostTarget = true;
                // 방향 설정
                if(agent.LostTargetPoint == Vector3.zero)
                    agent.LostTargetPoint = agent.target.transform.position;
                return State.Success;
            }
            return State.Failure;
        }
    }

}