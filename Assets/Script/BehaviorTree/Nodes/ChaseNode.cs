using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class ChaseNode : BT.Node
    {
        private NavMeshAgent agent;
        private EnemyAI ai;
        private Transform hidingObject;
        private System.Tuple<Vector3> v;

        // 이동 중 가까운 거리(1 ~ 2?) 내에 벽이 체크되면 벽에 붙어서 이동.
        // 벽으로 이동하기 전, 마지막 위치를 기억.
        // 벽으로 이동 후 Hide상태가 체크되며 마지막으로 기억한 위치로 재 탐색.
        // Ray에 체크되는 것이 없으면 체크되지 않는 방향으로 경계 후 기존의 이동하려는 위치로 이동함.

        public ChaseNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        // Ray를 쏴서 벽이 있는지 확인. 
        private void GodRay()
        {
            // 현재 바라보고 있는 방향의 좌 우 를 체크
            Vector3 origin = ai.transform.position + ai.transform.forward;
            Vector3 origin_R = origin + ai.transform.right;
            Vector3 origin_L = origin - ai.transform.right;

            Vector3 dir_R = (origin - origin_R).normalized;
            Vector3 dir_L = (origin - origin_L).normalized;

            RaycastHit hit;

            if(hidingObject == null)
            {
                if (Physics.Raycast(origin, dir_R, out hit, 2f))
                {
                    Debug.Log($"{hit.point}");
                    //ai.hide = true;
                    hidingObject = hit.transform;
                    // 목표 지점으로 이동하기인데, 목표지점을 어떻게 설정하냐?
                    ai.movingPoint = System.Tuple.Create(hit.point, false);
                    v = System.Tuple.Create(agent.destination);
                }
            }
            else
            {
                if (Physics.Raycast(origin, dir_R, out hit, 2f) )
                {

                }
                else
                {

                }

                if(Physics.Raycast(origin,dir_L, out hit, 2f))
                {

                }
            }
            //// 좌 우 방향으로 Ray를 쏴서 오브젝트가 있는지 체크
            //if(Physics.Raycast(origin, dir_L, 3f, LayerMask.GetMask("Wall")))
            //{
            //    Debug.Log("Wall_2");
            //}
            //else
            //{
            //    ai.hide = false;
            //}

        }


        public override NodeState Evaluate()
        {
            //GodRay();
            // 둘 다 없을 경우.
            if (ai.NowTarget == null && ai.movingPoint == null /*&& agent.hasPath == false*/) return NodeState.Failure;

            ai.SetColor(Color.yellow);
            Vector3 temp = ai.NowTarget == null ? ai.movingPoint.Item1 : ai.NowTarget.position;
            float distance = Vector3.Distance(temp, agent.transform.position);
            if(distance > 1f)
            {
                agent.isStopped = false;
                agent.SetDestination(temp);
                return NodeState.Running;
            }
            else
            {
                if (v != null)
                {
                    ai.movingPoint = System.Tuple.Create(v.Item1, false);
                    agent.SetDestination(ai.movingPoint.Item1);
                    return NodeState.Running;
                }
                agent.isStopped = true;
                ai.movingPoint = null;
                return NodeState.Success;
            }
        }
    }
}

