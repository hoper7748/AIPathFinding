using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    // 경계 노드 
    // 조건 - 추격이 아닌 이동으로 목표 위치에 도착하였는가?
    // 결과 - Yes - 좌 우 경계
    //      - No  - 계속 이동.
    // 무슨 일을 해야하나?
    // 주변 경계. 이동하는 방향을 기준으로 좌 우로 
    // 
    public class BoundaryNode : BT.Node
    {
        EnemyAI ai;
        NavMeshAgent agent;
        public BoundaryNode(EnemyAI _ai ,NavMeshAgent _agent)
        {
            ai = _ai;
            agent = _agent; 
        }

        public override NodeState Evaluate()
        {
            //if (agent.angularSpeed < 1) ;

            if (true)
                return NodeState.Running;
            return NodeState.Failure;
        }
    }
}
