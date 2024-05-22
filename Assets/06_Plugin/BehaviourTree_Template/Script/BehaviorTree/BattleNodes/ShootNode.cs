using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    /// <summary>
    /// ��ǥ�� ���� ����ϴ� ���
    /// 
    /// </summary>
    public class ShootNode : ActionNode
    {
        private float curTimer = 0;
        private float timer = 0.2f;
        public Color color;
        protected override void OnStart()
        {
            // �̰� �ٽ� �����غ���
            //pathFinding.PathRequestManager.RequestPath(agent.transform.position, agent.transform.position, agent.OnPathFound);
            //agent.navMeshAgent.SetDestination(agent.transform.position);
            agent.StopPathFinding();
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            agent.transform.LookAt(agent.target.transform);
            Shoot();
            return State.Success;   
        }

        // ������ ������ ��, ������Ʈ�� ������ ���� �ƴ� Ray�� ��� ������ ����.
        private void Shoot()
        {
            agent.ShootTarget();
        }

    }

}