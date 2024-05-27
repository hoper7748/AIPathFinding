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
                // ���� ��ġ�� �Ҿ���.
                agent.isLostTarget = true;
                // ���� ����
                if(agent.LostTargetPoint == Vector3.zero)
                    agent.LostTargetPoint = agent.target.transform.position;
                return State.Success;
            }
            return State.Failure;
        }
    }

}