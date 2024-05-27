using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// ã�ƶ�! ���� ��!!
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
                // ���� �� ������ ������.
                if (faraway == null)
                {
                    faraway = cols[i].transform;
                    distanceOld = Vector3.Distance(agent.transform.position, faraway.position);
                }
                else
                {
                    // �Ÿ��� ���ؼ� ���� �� ���� ����.
                    distanceOld = Vector3.Distance(agent.transform.position, faraway.position);
                    distanceNew = Vector3.Distance(agent.transform.position, cols[i].transform.position);
                    if (distanceNew <= distanceOld)
                    {
                        // �ִ�? �׷� ����
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
                    // �̰� Ÿ���� �Ѿư��� ��. Ÿ���� ��ġ�� �˰� �־����.
                    // Ÿ���� �Ҿ����� üũ�� �ʿ���.
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
