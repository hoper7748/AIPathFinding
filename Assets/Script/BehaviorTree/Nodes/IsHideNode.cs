using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    // �������� Ȯ���ϴ� ���.
    // �����ִ��� Ȯ���ϰ� ���� �ൿ�� ����
    // ���� �ൿ -> AmbushNode
    public class IsHideNode : BT.Node
    {
        EnemyAI ai;
        public IsHideNode(EnemyAI _ai)
        {
            ai = _ai;
        }

        public override NodeState Evaluate()
        {
            if (!ai.hide)
            {
                return NodeState.Failure;
            }

            if(ai.GetCurrentHealth() >= 100)
            {
                ai.getCurrentHealth = 100;
                ai.hide = false;

                return NodeState.Failure;
            }

            return NodeState.Success;
        }
    }
}
