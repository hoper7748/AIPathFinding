using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class IsDestinationNode : BT.Node
    {
        enum HideDirection
        {
            None = 0,
            Left = 1,
            Right = 2,
        };

        EnemyAI ai;
        NavMeshAgent agent;
        public IsDestinationNode(EnemyAI _ai, NavMeshAgent _agent )
        {
            ai = _ai;
            agent = _agent;
        }
        System.Tuple<Vector3> v;
        HideDirection h_Direction = HideDirection.None;


        // Ray를 쏴서 벽이 있는지 확인. 
        private void GetRay()
        {
            // 현재 바라보고 있는 방향의 좌 우 를 체크
            Vector3 origin = ai.transform.position + ai.transform.forward * 2f;
            Vector3 origin_R = ai.transform.position + ai.transform.forward * 2f + ai.transform.right * 2f;
            Vector3 origin_L = ai.transform.position + ai.transform.forward * 2f - ai.transform.right * 2f;

            Vector3 dir_R = (origin - origin_R).normalized;
            Vector3 dir_L = (origin - origin_L).normalized;

            RaycastHit hit;

            //Debug.DrawLine(ai.transform.position, origin);
            //Debug.DrawLine(origin, origin_R, Color.red);
            //Debug.DrawLine(origin, origin_L, Color.blue);

            if (HideDirection.None == h_Direction)
            {
                if (Physics.Raycast(origin, dir_L, out hit, 2f))
                {
                    h_Direction = HideDirection.Right;
                    ai.movingPoint = System.Tuple.Create(hit.point, false);
                    v = System.Tuple.Create(agent.destination);
                }
                if (Physics.Raycast(origin, dir_R, out hit, 2f))
                {
                    h_Direction = HideDirection.Left;
                    ai.movingPoint = System.Tuple.Create(hit.point, false);
                    v = System.Tuple.Create(agent.destination);
                }
            }
            else
            {
                if(ai.hide)
                {
                    switch (h_Direction)
                    {
                        case HideDirection.Left:
                            Debug.DrawLine(ai.transform.position, ai.transform.position - ai.transform.right);
                            // 옆에 벽이 없다면?
                            if (!Physics.Raycast(ai.transform.position, ai.transform.position - ai.transform.right,out hit, 2f))
                            {
                                ai.hide = false;
                                ai.movingPoint = null;
                                agent.isStopped = true;
                                h_Direction = HideDirection.None;
                                ai.transform.LookAt(ai.transform.position - ai.transform.right);
                            }
                            break;

                        case HideDirection.Right:

                            Debug.DrawLine(ai.transform.position, ai.transform.position + ai.transform.right);
                            // 옆에 벽이 없다면?
                            if (!Physics.Raycast(ai.transform.position, ai.transform.position + ai.transform.right, out hit, 2f))
                            {
                                ai.hide = false;
                                ai.movingPoint = null;
                                agent.isStopped = true;
                                h_Direction = HideDirection.None;
                                ai.transform.LookAt(ai.transform.position + ai.transform.right);
                            }
                            break;
                        default:

                            break;
                    }
                }
            }
        }
        public override NodeState Evaluate()
        {
            GetRay();
            switch (h_Direction)
            {
                case HideDirection.None:
                    return NodeState.Success;
                case HideDirection.Left:
                    break;
                case HideDirection.Right:
                    break;
                default:
                    break;
            }
            float a = Vector3.Distance(ai.transform.position, agent.destination);

            if (a < 0.5f)
            {
                ai.hide = true;
                ai.movingPoint = System.Tuple.Create(v.Item1, false);
            }

            if (!agent.hasPath && ai.movingPoint == null)
                return NodeState.Failure;

            return NodeState.Running;
        }
    }
}
