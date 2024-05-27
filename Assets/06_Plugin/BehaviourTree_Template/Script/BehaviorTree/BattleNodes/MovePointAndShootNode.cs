using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class MovePointAndShootNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            agent.StopPathFinding();
        }

        protected override State OnUpdate()
        {
            // ray를 쏴서 해당 방향에 벽이 존재하는지 체크 
            if (IsWallCheck())
            {
                // 플레이어가 시야각 안에 있다면 isLostTarget을 false로 변경해준다.
                agent.target = agent.FindViewTarget(30, 75, 1 << 9, 1 << 3);
                if(agent.target != null)
                {
                    agent.isLostTarget = false;
                    agent.LostTargetPoint = Vector3.zero;
                }
                
                if (CheckDistance() < 0.5f)
                {
                    return State.Success;
                }

                return State.Running;

                // 찾았다면 State.Success를 반환
                //GameObject target = agent.FindViewTarget(30f, 75f, 1 << 9, 1 << 3);
                //if (target == null)
                //{
                //    //Debug.Log("BB");
                //    return State.Running;
                //}
                ////Debug.Log("AA");
                //Debug.Log($"{target.transform.name}");
                //return State.Success;
            }
            return State.Success;
        }

        private bool IsWallCheck()
        {
            RaycastHit hit;
            //GameObject obj = Instantiate(new GameObject());
            //obj.transform.position = new Vector3(((agent.LostTargetPoint - agent.transform.position).normalized * 2f).x, 0, ((agent.LostTargetPoint - agent.transform.position).normalized * 2f).z) + agent.transform.position;
            Ray ray = new Ray(agent.transform.position, agent.LostTargetPoint - agent.transform.position);
            // 벽이 바라봐야할 방향을 가리고 있지 않다면 현재 바라보는 방향으로 공격한다.
            if (Physics.Raycast(ray, out hit, 5f, 1 << 9))
            {
                return true;
            }

            return false;
        }
        private float CheckDistance()
        {
            float distance = 0;
            try
            {
                distance = Vector3.Distance(agent.getPathPosition, agent.getLastPath);
                if (distance == 0)
                    throw new System.Exception(distance.ToString());
                return distance;
            }
            catch
            {
                Debug.Log("Can't Find Path.\ni Waiting new Path.");
                return 99;
            }
        }

        private void Movement()
        {

        }
    }
}