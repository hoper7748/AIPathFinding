using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

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

        
        bool isWall = false;
        bool isNearWall = false;
        System.Tuple<Vector3> originTarget;
        HideDirection h_Direction = HideDirection.None;


        // Ray�� ���� ���� �ִ��� Ȯ��. 
        private void GetRay()
        {
            int count = 0;
            // ���� �ٶ󺸰� �ִ� ������ �� �� �� üũ
            Vector3 origin = ai.transform.position + ai.transform.forward * 2f;
            Vector3 origin_R = ai.transform.position + ai.transform.forward * 2f + ai.transform.right * 2f;
            Vector3 origin_L = ai.transform.position + ai.transform.forward * 2f - ai.transform.right * 2f;

            Vector3 dir_R = (origin - origin_R).normalized;
            Vector3 dir_L = (origin - origin_L).normalized;

            RaycastHit Rhit;

            if (HideDirection.None == h_Direction)
            {
                if (Physics.Raycast(origin, dir_L, out Rhit, 2f))
                {
                    h_Direction = HideDirection.Right;
                    ai.movingPoint = System.Tuple.Create(Rhit.point);
                    originTarget = System.Tuple.Create(agent.destination);
                }
                if (Physics.Raycast(origin, dir_R, out Rhit, 2f))
                {
                    h_Direction = HideDirection.Left;
                    ai.movingPoint = System.Tuple.Create(Rhit.point);
                    originTarget = System.Tuple.Create(agent.destination);
                }
            }
            else
            {
                //switch (h_Direction)
                //{
                //    case HideDirection.Left:
                //        // ���� ���� ���ٸ�?
                //        if (!Physics.Raycast(ai.transform.position, ai.transform.position - ai.transform.right,out Rhit, 2f))
                //        {
                //            agent.isStopped = true;
                //            h_Direction = HideDirection.None;
                //        }
                //        break;

                //    case HideDirection.Right:
                //        // ���� ���� ���ٸ�?
                //        if (!Physics.Raycast(ai.transform.position, ai.transform.position + ai.transform.right, out Rhit, 2f))
                //        {
                //            // ���� ���� �̵��� �� ���� ������ �ִ� ��� ���� �缳��
                //            agent.isStopped = true;
                //            h_Direction = HideDirection.None;
                //        }
                //        break;
                //    default:
                //         break;
                //}
            }
        }

        
        private void checkUpdate()
        {
            Vector3 origin = ai.transform.position + ai.transform.forward * 2f;
            Vector3 origin_R = ai.transform.position + ai.transform.forward * 2f + ai.transform.right * 2f;
            Vector3 origin_L = ai.transform.position + ai.transform.forward * 2f - ai.transform.right * 2f;

            Vector3 dir_R = (origin - origin_R).normalized;
            Vector3 dir_L = (origin - origin_L).normalized;

            RaycastHit Rhit;

            if (HideDirection.None == h_Direction)
            {
                if (Physics.Raycast(origin, dir_L, out Rhit, 2f))
                {
                    h_Direction = HideDirection.Right;
                    ai.movingPoint = System.Tuple.Create(Rhit.point);
                    originTarget = System.Tuple.Create(agent.destination);
                }
                if (Physics.Raycast(origin, dir_R, out Rhit, 2f))
                {
                    h_Direction = HideDirection.Left;
                    ai.movingPoint = System.Tuple.Create(Rhit.point);
                    originTarget = System.Tuple.Create(agent.destination);
                }
            }
            //else if(HideDirection.Left == h_Direction)
            //{
            //    if(!Physics.Raycast(ai.transform.position, ai.transform.position - ai.transform.right, out Rhit, 2f))
            //    {
            //        agent.isStopped = true;
            //        h_Direction = HideDirection.None;
            //    }
            //}
            //else if(HideDirection.Left == h_Direction)
            //{
            //    if (!Physics.Raycast(ai.transform.position, ai.transform.position + ai.transform.right, out Rhit, 2f))
            //    {
            //        // ���� ���� �̵��� �� ���� ������ �ִ� ��� ���� �缳��
            //        agent.isStopped = true;
            //        h_Direction = HideDirection.None;
            //    }
            //}
        }

        // how check
        // ���� ������ ���� ��� �������� ���� �ٰ����� ���� ���� �� �������� ��� üũ�ؾ��ϴµ�, ���������� �پ ȸ���ϴ� �������� üũ�ϴ� ������ �����ʿ��� �������� �ٲ�� �� �׷��鼭 ���� ������
        // �̰� ���� ������ ���� �ع��� ã�ƺ��� �ϴ� �ڰ�!
        // ���� ��ġ���� ���������� Ray�� ���� �ɸ��� �ɸ��� ���� �پ �̵���.
        public void WallCheck()
        {
            // shoot Ray and check
            RaycastHit hit;

            // 3m - check wall
            GameObject Wall = ai.FindViewTarget(2f, 1 << 10);

            Vector3 horizontalLeftDir = EnemyAI.AngleToDirY(ai.transform, -90f * .5f);
            Vector3 horizontalRightDir = EnemyAI.AngleToDirY(ai.transform, 90f * .5f);

            if (Physics.Raycast(ai.transform.position, horizontalLeftDir, out hit, 3f) || Physics.Raycast(ai.transform.position, horizontalRightDir, out hit, 3f))
            {
                originTarget = System.Tuple.Create(ai.movingPoint.Item1);
                ai.movingPoint = System.Tuple.Create(hit.point);
                isWall = true;
            }
        }

        private void NearTheWall()
        {
            RaycastHit hit;

            // 3m - check wall
            GameObject Wall = ai.FindViewTarget(2f, 1 << 10);

            Vector3 horizontalLeftDir = EnemyAI.AngleToDirY(ai.transform, -90f * .5f);
            Vector3 horizontalRightDir = EnemyAI.AngleToDirY(ai.transform, 90f * .5f);

            if (!Physics.Raycast(ai.transform.position, horizontalLeftDir, out hit, 3f) && !Physics.Raycast(ai.transform.position, horizontalRightDir, out hit, 3f))
            {
                isNearWall = false;
            }
        }

        public bool HavingMoivngPointNearWall()
        {
            // ���� ���� ��� �پ��ִٴ� ���� ��� üũ �� ���ΰ�?
            if (ai.movingPoint != null)
            {
                if (!isWall && !isNearWall)
                    WallCheck();
                else if (isNearWall)
                    NearTheWall();
                else
                {
                    // �Ÿ��� Ȯ���ؼ� ���� �پ��ٸ�? ���� ����� ���� üũ �ؾ���. ��� üũ�� �ؾ� �ұ�?
                    float distance = Vector3.Distance(ai.transform.position, agent.destination);

                    if (distance <= 0.5f)
                    {
                        isWall = false;
                        isNearWall = true;
                        ai.movingPoint = System.Tuple.Create(originTarget.Item1);
                    }
                }
                return true;
            }
            return false;
        }


        public override NodeState Evaluate()
        {
            if (ai.NowTarget != null && ai.lostTarget == false)
            {
                ai.lostTarget = true;
                ai.LostTarget().Forget();
                return NodeState.Running;
            }
            else
            {
                #region Legarcy

                //    switch (h_Direction)
                //    {
                //        case HideDirection.None:
                //            return NodeState.Success;
                //        case HideDirection.Left:
                //            break;
                //        case HideDirection.Right:
                //            break;
                //        default:
                //            break;
                //    }
                //    float a = Vector3.Distance(ai.transform.position, agent.destination);

                //    if (a <= 0.5f)
                //    {
                //        //ai.hide = true;
                //        ai.movingPoint = null;
                //        //return NodeState.Success;
                //        //ai.movingPoint = System.Tuple.Create(v.Item1, false);
                //    }

                //}

                //return NodeState.Running;

                #endregion
                if (HavingMoivngPointNearWall())
                    return NodeState.Success;

                if (!agent.hasPath && ai.movingPoint == null)
                    return NodeState.Failure;
            }
            return NodeState.Success;
        }
    }

}
