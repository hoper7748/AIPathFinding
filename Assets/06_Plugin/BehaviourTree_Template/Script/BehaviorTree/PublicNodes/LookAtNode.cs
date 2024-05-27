using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{

    public class LookAtNode : ActionNode
    {
        Vector3 dir;
        float distance = 0;
        protected override void OnStart()
        {
            // lookPosition을 바라보자 
            dir = agent.LookPosition - agent.transform.position;
            distance = Vector3.Distance(agent.transform.position, agent.LookPosition);
            //Debug.Log($"{distance}");
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // 뭔가 예상치 못한 문제가 생기면 새로운 작업을 하러 감.
            if (false)
                return State.Failure;

            // 바라보게 할 것인데... 봤다는 것을 체크하려면 Ray를 쏴야겠다.
            if(ForwradCheck())
            {
                return State.Success;
            }
            return State.Running;   
        }

        // ray를 쏴서 체크 할 것
        private bool ForwradCheck()
        {
            // Ray를 쏴서 벽을 마주볼 때 까지 체크
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation(dir), 3 * Time.deltaTime);
            if (Physics.Raycast(agent.transform.position, agent.transform.forward, distance * 1.5f, 1<< 9 ))
            {
                return true;
            }
            return false;
        }

    }
}