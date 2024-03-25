using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class ShootNode : BT.Node
    {
        // ������ �ϱ� ���� ����.
        private NavMeshAgent agent;
        private EnemyAI ai;
        private GameObject bullet;

        public ShootNode(NavMeshAgent _agent, EnemyAI _ai, GameObject _bullet, string _name)
        {
            agent = _agent;
            ai = _ai;
            bullet = _bullet;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log($"{name}");
            // n�ʴ� źȯ �߻�
            // ĳ���� �� �ʿ䰡 �ִ°�?

            agent.isStopped = true;
            ai.SetColor(Color.green);
            if(bullet == null)
            {
                Debug.Log("Not Have Bullet");
                return NodeState.Failure;
            }
            return NodeState.Success;
        }
    }
}
