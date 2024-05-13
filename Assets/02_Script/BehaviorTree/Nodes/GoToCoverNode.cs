using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class GoToCoverNode : BT.Node
    {

        private NavMeshAgent agent;
        private EnemyAI ai;

        public GoToCoverNode(NavMeshAgent _agent, EnemyAI _ai, string _name)
        {
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log($"{name}");
            Transform coverSpot = ai.GetBestCoverSpot();
            if (coverSpot == null)
                return NodeState.Failure;

            ai.SetColor(Color.blue);
            float distance = Vector3.Distance(coverSpot.position, agent.transform.position);
            if(distance > 0.5f)
            {
                agent.isStopped = false;
                agent.SetDestination(coverSpot.position);
                
                return NodeState.Running;
            }
            else
            {
                ai.isHide = true;
                agent.isStopped = true;
                // ���ƺ��� �ϴµ�...
                //agent.angularSpeed = 0;
                // ���Ϸ� ��ȯ�� ���ؼ� �׷��� 
                Vector3 eular = ai.transform.rotation.eulerAngles;
                ai.transform.rotation = Quaternion.Euler(0, eular.y + 180f, 0);

                return NodeState.Success;
            }
        }

        private IEnumerator AA()
        {
            yield return null;
        }
    }
}

