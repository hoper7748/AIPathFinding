using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// 찾아라! 숨을 곳!!
    /// </summary>
    public class IsCovereAvaliableNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            Transform bestSpot = FindCover();
            if (bestSpot == null)
                return State.Failure;

            pathFinding.PathRequestManager.RequestPath(new pathFinding.PathRequest(agent.transform.position, bestSpot.position, agent.OnPathFound));

            return bestSpot != null ? State.Success : State.Failure;
        }

        private Transform FindCover()
        {
            Transform faraway = null;
            float distanceOld = 0.0f;
            float distanceNew = 0.0f;

            Collider[] cols = Physics.OverlapSphere(agent.transform.position, 5f, 1 << 9);
            for (int i = 0; i < cols.Length; i++)
            {
                // 가장 먼 곳으로 가야함.
                if (faraway == null)
                {
                    faraway = cols[i].transform;
                    distanceOld = Vector3.Distance(agent.transform.position, faraway.position);
                }
                else
                {
                    // 거리를 비교해서 가장 먼 곳에 간다.
                    distanceOld = Vector3.Distance(agent.transform.position, faraway.position);
                    distanceNew = Vector3.Distance(agent.transform.position, cols[i].transform.position);
                    if (distanceNew <= distanceOld)
                    {
                        // 멀다? 그럼 변경
                        faraway = cols[i].transform;
                        distanceOld = distanceNew;
                    }
                }
            }

            float minAngle = 90;

            if (faraway != null)
            {
                faraway = FindBestSpotInCover(faraway.GetComponent<BT.Cover>(), ref minAngle);
            }

            return faraway;
        }

        private Transform FindBestSpotInCover(BT.Cover cover, ref float minAngle)
        {
            Transform bestSpot = null;
            try
            {
                Transform[] avaliableSpots = cover.GetCoverSpots();
                for (int i = 0; i < avaliableSpots.Length; i++)
                {
                    // 이건 타겟으 쫓아가는 것. 타겟의 위치는 알고 있어야함.
                    // 타겟을 잃었는지 체크가 필요함.
                    Vector3 direction = agent.target.transform.position - avaliableSpots[i].position;
                    if (CheckIfSpotIsValid(avaliableSpots[i]))
                    {
                        float angle = Vector3.Angle(avaliableSpots[i].forward, direction);

                        if (angle < minAngle)
                        {
                            minAngle = angle;
                            bestSpot = avaliableSpots[i];
                        }
                    }
                }
            }
            catch
            {
                Debug.LogWarning("Error");
                agent.LostTargetPoint = Vector3.zero;
                agent.isLostTarget = true;
                agent.isEncounter = false;
            }
            return bestSpot;
        }

        private bool CheckIfSpotIsValid(Transform spot)
        {
            RaycastHit hit;

            Vector3 direction = agent.target.transform.position - spot.position;

            if (Physics.Raycast(spot.position, direction, out hit))
            {
                if (hit.collider.transform != agent.target)
                {
                    return true;
                }
            }
            return false;
        }

    }


}
