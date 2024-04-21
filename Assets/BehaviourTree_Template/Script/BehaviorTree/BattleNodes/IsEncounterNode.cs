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
            if(agent.target != null)
            {
                return State.Success;
            }
            return State.Failure;
        }
    }

}