using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
            readyTimer = Random.Range(1f, 2f);
        }

        public override NodeState Evaluate()
        {
            // �ƹ��͵� ���� �ʴ� ����.
            // ��ǥ�� Ž��
            ai.SetColor(Color.cyan);
            curTimer += Time.deltaTime;
            if (curTimer > readyTimer && ai.movingPoint == null)
            {
                agent.angularSpeed = 120;
                // ���� �ð��� �������Ƿ� ���ο� ��ġ Ž�� ����.
                RandomPointSearch();
                readyTimer = Random.Range(1f, 2f);
                curTimer = 0;
                return NodeState.Success;
            }
            return NodeState.Failure;
        }

        // ���ο� ��ǥ�� ��ġ�ϰ� �ϴ� �ż���
        public bool RandomPointSearch()
        {
            NavMeshHit hit;
            Transform tf = ai.WayPoints[Random.Range(0, ai.WayPoints.Length)];
            
            for(int i = 0; i < 30; i ++)
            {
                Vector3 randomPoint = tf.position + Random.insideUnitSphere * 2.5f;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    ai.movingPoint = System.Tuple.Create(hit.position, false);
                    return true;
                }
            }
            return false;
        }
    }
}