using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class RandomWaitNode : ActionNode
    {
        public float min;
        public float max;

        // ´ë±â Áß
        float curTimer;
        float randWaitTime;

        protected override void OnStart()
        {
            randWaitTime = Random.Range(min, max);
            curTimer = 0;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            curTimer += Time.deltaTime; 
            if(curTimer >= randWaitTime)
            {
                return State.Success;
            }

            return State.Running;
        }
    }

}