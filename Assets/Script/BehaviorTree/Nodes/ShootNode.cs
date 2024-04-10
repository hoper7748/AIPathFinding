using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Cysharp.Threading.Tasks;
namespace BT
{
    public class ShootNode : BT.Node
    {
        // 공격을 하기 위한 수단.
        private NavMeshAgent agent;
        private EnemyAI ai;
        private Bullet bullet;
        private float curTimer;
        private Transform shootPos;
        private Transform prevTarget;

        public ShootNode(NavMeshAgent _agent, EnemyAI _ai, Bullet _bullet, Transform _shootPos, string _name)
        {
            agent = _agent;
            ai = _ai;
            bullet = _bullet;
            name = _name;
            shootPos = _shootPos;
        }

        public override NodeState Evaluate()
        {
            ai.lostTarget = false;
            agent.isStopped = true;
            ai.SetColor(Color.green);
            if (bullet == null)
            {
                Debug.Log("Not Have Bullet");
                return NodeState.Failure;
            }
            else
            {
                // ai.nowtarget에 공격
                // 3번? 
                curTimer += Time.deltaTime;
                Vector3 lookRotation = agent.steeringTarget - ai.transform.position;
                ai.transform.rotation = Quaternion.Lerp(ai.transform.rotation, Quaternion.LookRotation(lookRotation), 5f * Time.deltaTime);
                if (curTimer > 2f)
                {
                    LoopB().Forget();
                    curTimer = 0;
                }
            }
            return NodeState.Success;
        }
        
        public IEnumerator LoopA()
        {
            for(int i =0; i < 3; i++)
            {
                Debug.Log("coroutine 땅!");
                Shoot();
                yield return new WaitForSeconds(0.25f);
            }
        }

        private async UniTaskVoid LoopB()
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.Log("async 땅!");
                Shoot();
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }
        }

        private void Shoot()
        {
            Vector3 pos;
            if (ai.NowTarget == null)
            {
                pos = prevTarget.position + (prevTarget.forward * 3.5f) + UnityEngine.Random.insideUnitSphere * 1f;
            }
            else
            {
                Vector3 v1 = ai.NowTarget.position + (UnityEngine.Random.insideUnitSphere * 1f);
                Vector3 v2 = ai.NowTarget.position + (ai.NowTarget.forward * 3.5f) + UnityEngine.Random.insideUnitSphere * 1f;
                pos = ai.NowTarget.GetComponent<EnemyAI>().NowTarget == ai.transform ? v1 : v2;
                prevTarget = ai.NowTarget;
            }
            Bullet bullet = GameObject.Instantiate(this.bullet, shootPos.position, Quaternion.identity);
            bullet.ShootTarget(ai.transform, pos, ai.transform.position, 1f, 0.5f);
            GameObject.Destroy(bullet.gameObject, 2f);
        }
    }
}
