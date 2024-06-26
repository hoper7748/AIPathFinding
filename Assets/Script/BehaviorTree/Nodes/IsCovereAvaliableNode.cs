using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class IsCovereAvaliableNode : Node
    {
        
        private Cover[] avaliableCovers;
        private Transform target;
        private EnemyAI ai;

        public IsCovereAvaliableNode(Cover[] _avaliableCovers, Transform _target, EnemyAI _ai, string _name)
        {
            avaliableCovers = _avaliableCovers;
            target = _target;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            Debug.DrawLine(ai.transform.position, ai.transform.forward * 10f);
            //Debug.Log($"{name}");
            Transform bestSpot = FindBestCoverSpot();
            ai.SetBestCoverSopt(bestSpot);
            return bestSpot != null ? NodeState.Success : NodeState.Failure; 
        }

        private Transform FindBestCoverSpot()
        {
            // 새로운 경로 찾기
            // 이미 도달해야할 목표가 있다?
            if(ai.GetBestCoverSpot() != null)
            {
                // 목표 지점으로 가는 길에 상대방이 존재.
                if(ai.FindViewTarget(30f) == null)
                {
                    Debug.Log("AA");
                    return ai.GetBestCoverSpot();
                }
            }
            float minAngle = 90;
            Transform bestSpot = null;
            for(int i =0; i < avaliableCovers.Length; i++)
            {
                Transform bestSpotInCover = FindBestSpotInCover(avaliableCovers[i], ref minAngle);
                if(bestSpotInCover != null)
                    bestSpot = bestSpotInCover;
            }
            return bestSpot;
        }

        private Transform FindBestSpotInCover(Cover cover, ref float minAngle)
        {
            Transform[] avaliableSpots = cover.GetCoverSpots();
            Transform bestSpot = null;
            for(int i =0; i < avaliableSpots.Length;i++)
            {
                Vector3 direction = target.position - avaliableSpots[i].position;
                if (CheckIfSpotIsValid(avaliableSpots[i]))
                {
                    float angle = Vector3.Angle(avaliableSpots[i].forward, direction);

                    if(angle < minAngle)
                    {
                        minAngle = angle; 
                        bestSpot = avaliableSpots[i];
                    }
                }
            }
            return bestSpot;
        }

        // 가는 방향에 적이 있을 경우.
        public bool CheckIfPlayerToWay(Transform spot)
        {
            RaycastHit hit;
            Vector3 direction = ai.transform.forward;
            
            if(Physics.Raycast(spot .position, direction, out hit))
            {
                if(hit.collider.transform != target)
                {
                    return true;
                }
            }

            return false;
        }


        private bool CheckIfSpotIsValid(Transform spot)
        {
            RaycastHit hit;
            // target to spot pos;
            Vector3 direction = target.position - spot.position;

            if(Physics.Raycast(spot.position, direction, out hit))
            {
                if(hit.collider.transform != target)
                { 
                    return true; 
                }
            }
            return false;
        }

    }
}
