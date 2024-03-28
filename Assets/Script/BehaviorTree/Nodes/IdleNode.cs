using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    /// <summary>
    /// IdleState Node������ �۵�?
    /// ��ȯ ��ÿ� �ƹ��� �۵��� ���� �ʱ� ������ �ؾ� �� ���� ��������
    /// �׷��� �ؾ��� ���� �����̳�?
    /// 1. ���� ���ظ� �Ծ��°�? �׷� ���ظ� ���� ��ġ�� ����ΰ�?
    /// 2. ���� �̵��� ��ġ ����.
    /// </summary>
    public class IdleNode : BT.Node
    {
        // ���� �� �ƹ��� ���µ� �ƴϱ� ������ ��⸦ �ϴ� �����̸� ��� ���°� ���������� ���� �� �۵��ϴ� ��� Node�� ������ �ܰ�.
        EnemyAI ai;
        NavMeshAgent agent;
        float readyTimer = 0;
        float curTimer = 0;


        public IdleNode(EnemyAI _ai, NavMeshAgent _agent)
        {
            ai = _ai;
            agent = _agent;
            // �����ǰ� ���� �ൿ������ ��� �ð�.
            readyTimer = Random.Range(1f, 5f);
        }

        public override NodeState Evaluate()
        {
            // �ƹ��͵� ���� �ʴ� ����.
            // ��ǥ�� Ž��

            if (agent.hasPath)
                return NodeState.Failure;

            ai.SetColor(Color.cyan);

            curTimer += Time.deltaTime;
            if(curTimer > readyTimer)
            {
                // ���� �ð��� �������Ƿ� ���ο� ��ġ Ž�� ����.
                RandomPointSearch();
                curTimer = 0;
            }

            return NodeState.Running;
        }

        // ���ο� ��ǥ�� ��ġ�ϰ� �ϴ� �ż���
        public bool RandomPointSearch()
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(ai.transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                for(int i = 0; i < 30; i++)
                {
                    //Tuple<Vector3, bool> a = (Vector3 a = hit.position, bool b = true);
                    //ai.movingPoint = ;
                    return true;
                }
            }
            return false;
            
        }


    }

}