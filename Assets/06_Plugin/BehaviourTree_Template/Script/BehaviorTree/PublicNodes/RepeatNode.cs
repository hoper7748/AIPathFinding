using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class RepeatNode : DecoratorNode
    {
        public bool Loop = true;
        public int count = 0;
        int curCount;
        protected override void OnStart()
        {
            curCount = 0;
        }

        protected override void OnStop()
        {

        }


        protected override State OnUpdate()
        {
            if(Loop)
            {
                child.Update();
                return State.Running;
            }
            else
            {
                switch (child.Update())
                {
                    case State.Running:
                        break;
                    case State.Failure:
                        // 들어오긴 했으니 완료 
                        return State.Success;
                    case State.Success:
                        curCount++;
                        break;
                    default:
                        break;
                }
                return curCount == count ? State.Success : State.Running;
            }
        }
    }
}
