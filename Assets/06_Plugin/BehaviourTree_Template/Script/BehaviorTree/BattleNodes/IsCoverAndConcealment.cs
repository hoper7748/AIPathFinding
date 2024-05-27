using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class IsCovverAndConcealment : ActionNode
    {
        // ���������� Ȯ�� �� ���⿡ ���� ���� �� ���� �̵�
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {
            // ray�� ���� �ش� ���⿡ ���� �����ϴ��� üũ 
            if (IsWallCheck())
            {
                // �̵� �ǽ�
                return State.Running;
            }
            return State.Success;
        }

        private void Movement()
        {
;

        }

        private bool IsWallCheck()
        {
            RaycastHit hit;
            Ray ray = new Ray(agent.transform.position, agent.transform.position - agent.LostTargetPoint );
            // ���� �ٶ������ ������ ������ ���� �ʴٸ� ���� �ٶ󺸴� �������� �����Ѵ�.
            if(Physics.Raycast(ray, out hit, 2f, 1 << 9))
            {
                return true;
            }

            return false;
        }
    }
}