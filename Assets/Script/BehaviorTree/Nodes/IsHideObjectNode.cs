using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    // 숨을 수 있는 오브젝트가 있는지 체크하는 노드 -> chase하기 전에 체크를 함.
    public class IsHideObjectNode : BT.Node
    {
        enum HideDirection
        {
            None = 0,
            Left = 1,
            Right = 2,
        };

        EnemyAI ai;
        NavMeshAgent agent;
        System.Tuple<Vector3> v;
        HideDirection h_Direction = HideDirection.None;
        public IsHideObjectNode(EnemyAI _ai, NavMeshAgent _agent)
        {
            ai = _ai;
            agent = _agent;
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

            if(HideDirection.None == h_Direction)
            {
                if (Physics.Raycast(origin, dir_R, out hit, 2f))
                {
                    h_Direction = HideDirection.Right;
                    ai.movingPoint = System.Tuple.Create(hit.point, false);
                    v = System.Tuple.Create(agent.destination);
                }
                if (Physics.Raycast(origin, dir_L, out hit, 2f))
                {
                    h_Direction = HideDirection.Left;
                    ai.movingPoint = System.Tuple.Create(hit.point, false);
                    v = System.Tuple.Create(agent.destination);
                }
            }
            else
            {
                switch (h_Direction)
                {
                    case HideDirection.Left:
                        {
                            if(!Physics.Raycast(origin, dir_L, out hit, 2 ))
                            {
                                // 대상이 없으면? 
                                h_Direction = HideDirection.None;
                                ai.movingPoint = System.Tuple.Create(v.Item1, false);
                            }
                            break;
                        }
                    case HideDirection.Right:
                        {
                            if (!Physics.Raycast(origin, dir_R, out hit, 2))
                            {
                                // 대상이 없으면? 
                                h_Direction = HideDirection.None;
                                ai.movingPoint = System.Tuple.Create(v.Item1, false);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public override NodeState Evaluate()
        {
            GodRay();

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
            if(Vector3.Distance(ai.transform.position, agent.destination) < 1)
            {
                ai.movingPoint = System.Tuple.Create(v.Item1, false);
            }
            return NodeState.Running;
            throw new System.NotImplementedException();
        }
    }
}
