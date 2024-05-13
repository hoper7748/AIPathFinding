using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class GoToCoverNode : ActionNode
    {
        protected override void OnStart()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStop()
        {
            throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {
            Transform coverSpot = agent.GetBestCoverSpot();
            if (coverSpot == null)
            {
                return State.Failure;
            }

            float distance = Vector3.Distance(coverSpot.position, agent.transform.position);
            if(distance > 0.5f)
            {
                //agent.StopPathFinding();
                pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(agent.transform.position, coverSpot.position, agent.OnPathFound));
                //agent.navMeshAgent.isStopped = false;
                //agent.navMeshAgent.SetDestination(coverSpot.position);

                return State.Running;
            }

            return State.Failure;
        }

        //private void Temp()
        //{
        //    Transform coverSpot = agent.GetBestCoverSpot();
        //    if (coverSpot == null) ;
        //        //return State.Failure;

        //    //ai.SetColor(Color.blue);
        //    float distance = Vector3.Distance(coverSpot.position, agent.transform.position);
        //    if (distance > 0.5f)
        //    {
        //        pathFinding.PathRequestManager.RequestPath(agent.transform.position, coverSpot.position, agent.OnPathFound);
        //        //agent.navMeshAgent.isStopped = false;
        //        //agent.navMeshAgent.SetDestination(coverSpot.position);

        //        //return State.Running;
        //    }
        //    else
        //    {
        //        agent.StopPathFinding();
        //        //ai.isHide = true;
        //        //agent.navMeshAgent.isStopped = true;
        //        // 돌아봐야 하는데...
        //        //agent.angularSpeed = 0;
        //        // 오일러 변환은 안해서 그랬네 
        //        Vector3 eular = agent.transform.rotation.eulerAngles;
        //        agent.transform.rotation = Quaternion.Euler(0, eular.y + 180f, 0);

        //        //return State.Success;
        //    }
        //}
    }
}
