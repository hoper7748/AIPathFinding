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

        // �̵� �� ����� �Ÿ�(1 ~ 2?) ���� ���� üũ�Ǹ� ���� �پ �̵�.
        // ������ �̵��ϱ� ��, ������ ��ġ�� ���.
        // ������ �̵� �� Hide���°� üũ�Ǹ� ���������� ����� ��ġ�� �� Ž��.
        // Ray�� üũ�Ǵ� ���� ������ üũ���� �ʴ� �������� ��� �� ������ �̵��Ϸ��� ��ġ�� �̵���.

        public ChaseNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        // Ray�� ���� ���� �ִ��� Ȯ��. 
        private void GodRay()
        {
            // ���� �ٶ󺸰� �ִ� ������ �� �� �� üũ
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
                    // ��ǥ �������� �̵��ϱ��ε�, ��ǥ������ ��� �����ϳ�?
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
            //// �� �� �������� Ray�� ���� ������Ʈ�� �ִ��� üũ
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
            // �� �� ���� ���.
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

