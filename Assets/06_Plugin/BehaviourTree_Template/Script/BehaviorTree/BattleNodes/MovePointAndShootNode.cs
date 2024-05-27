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
            // ray�� ���� �ش� ���⿡ ���� �����ϴ��� üũ 
            if (IsWallCheck())
            {
                // �÷��̾ �þ߰� �ȿ� �ִٸ� isLostTarget�� false�� �������ش�.
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

                // ã�Ҵٸ� State.Success�� ��ȯ
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
            // ���� �ٶ������ ������ ������ ���� �ʴٸ� ���� �ٶ󺸴� �������� �����Ѵ�.
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