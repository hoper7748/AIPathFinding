using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class IsCoverAndConcealment : ActionNode
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
            return State.Success;
        }

        private void Movement()
        {
;

        }

    }
}