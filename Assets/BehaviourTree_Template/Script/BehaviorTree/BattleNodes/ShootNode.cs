using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    /// <summary>
    /// 목표를 향해 사격하는 노드
    /// 
    /// </summary>
    public class ShootNode : ActionNode
    {
        private float curTimer = 0;
        private float timer = 0.2f;
        public Color color;
        protected override void OnStart()
        {
            agent.navMeshAgent.SetDestination(agent.transform.position);
            //agent.
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if(false)
            {
                return State.Failure;
            }
            //curTimer += Time.deltaTime;

            //if (curTimer <= timer)
            //{
            //    Shoot();
            //    return State.Running;
            //}
            //curTimer = 0;
            return State.Success;
            //return State.Failure;
        }

        // 공격을 진행할 때, 오브젝트를 날리는 것이 아닌 Ray를 쏘는 것으로 하자.
        private void Shoot()
        {
            //Debug.DrawLine(agent.transform.position, agent.target.transform.position, color, 5f);
            //agent.shoot = true;
        }

    }

}