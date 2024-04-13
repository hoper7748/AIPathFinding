using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace BT
{
    /// <summary>
    /// IdleState Node에서의 작동?
    /// 소환 당시엔 아무런 작동도 하지 않기 때문에 해야 할 일이 정해진다
    /// 그래서 해야할 일이 무엇이냐?
    /// 1. 내가 피해를 입었는가? 그럼 피해를 받은 위치는 어디인가?
    /// 2. 다음 이동할 위치 지정.
    /// </summary>
    public class IdleNode : BT.Node
    {
        // 시작 시 아무런 상태도 아니기 때문에 대기를 하는 상태이며 모든 상태가 진행중이지 않을 때 작동하는 모든 Node의 마지막 단계.
        EnemyAI ai;
        NavMeshAgent agent;
        float readyTimer = 0;
        float curTimer = 0;


        public IdleNode(EnemyAI _ai, NavMeshAgent _agent)
        {
            ai = _ai;
            agent = _agent;
            // 스폰되고 다음 행동까지의 대기 시간.
            readyTimer = Random.Range(1f, 2f);
        }

        public override NodeState Evaluate()
        {
            // 아무것도 하지 않는 상태.
            // 목표물 탐색
            ai.SetColor(Color.cyan);
            curTimer += Time.deltaTime;
            if (curTimer > readyTimer && ai.movingPoint == null)
            {
                agent.angularSpeed = 120;
                // 일정 시간이 지났으므로 새로운 위치 탐색 시작.
                RandomPointSearch();
                readyTimer = Random.Range(1f, 2f);
                curTimer = 0;
                return NodeState.Success;
            }
            return NodeState.Failure;
        }

        // 새로운 좌표를 서치하게 하는 매서드
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