using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace BT
{
    public class IsCovereAvaliableNode : Node
    {
        // ���⼭ BestCoverSpot�� ã�´�.

        
        private Cover[] avaliableCovers;
        //private Transform target;
        private EnemyAI ai;
        private NavMeshAgent agent;

        public IsCovereAvaliableNode(Cover[] _avaliableCovers, Transform _target, EnemyAI _ai, NavMeshAgent _agent, string _name)
        {
            avaliableCovers = _avaliableCovers;
            agent = _agent;
            ai = _ai;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            // ������ �̷��� ã���� �ȵ�!!!!
            // ���� �߰��� �ƴ°� ���� �����Ͽ� �پ��� ����� ���� �־���.

            // ���� �ٶ󺸴� ���⿡ ���� �ִ°�?
            GameObject targets = ai.FindViewTarget(30f, 1 << 9);
            if (ai.GetBestCoverSpot() == null || targets != null)
            {
                Transform bestSpot = FindCover();
                ai.SetBestCoverSopt(bestSpot);
                return bestSpot != null ? NodeState.Success : NodeState.Failure;
            }
            return NodeState.Running;
        }
        private Transform FindCover()
        {
            Collider[] cols = Physics.OverlapSphere(ai.transform.position, 20f);
            Transform Temp = null;
            Transform faraway = null;
            float distanceOld = 0.0f;
            float distanceNew = 0.0f;
            for (int i = 0; i < cols.Length;  i++)
            {
                // ���� �� ������ ������.
                if(faraway == null)
                {
                    faraway = cols[i].transform;
                    distanceOld = Vector3.Distance(ai.transform.position, faraway.position);
                }
                else
                {
                    // �Ÿ��� ���ؼ� ���� �� ���� ����.
                    distanceOld = Vector3.Distance(ai.transform.position, faraway.position);
                    distanceNew = Vector3.Distance(ai.transform.position, cols[i].transform.position);
                    if(distanceNew >= distanceOld)
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
                faraway = FindBestSpotInCover(faraway.GetComponent<Cover>(), ref minAngle);
            }

            //GameObject obj = new GameObject();
            //obj.transform.position = faraway == null ? Vector3.zero : faraway.position;
            return faraway;
            
        }

        private Transform FindBestCoverSpot()
        {
            // ���ο� ��� ã��
            // �̹� �����ؾ��� ��ǥ�� �ִ�?
            if(ai.GetBestCoverSpot() != null)
            {
                // ��ǥ �������� ���� �濡 ������ ����.
                if(ai.FindViewTarget(30f, 1 << 9) == null)
                    return ai.GetBestCoverSpot();
            }
            // Spotã�� �˰����� ����
            // ��ü�� �߽����� �� ������ Ž���Ͽ� ã�� ��ҵ鿡 �ڽ�Ʈ�� �ű�� ���������� ���� ����� ���� �����Ͽ� ����ϰ� ��.

            #region TestCode
            ////LayerMask a = 1 << 9;
            //Collider[] colliders = Physics.OverlapSphere(ai.transform.position, 30f, 1 << 9);
            ////agent.destination 
            //// ��ǥ����, �ڽ�Ʈ
            ////List<System.Tuple<Transform, float>> bestSpots = new List<System.Tuple<Transform, float>>();
            //Dictionary<Transform, float> spotcosts = new Dictionary<Transform, float>();
            //Transform[] spots = null;
            //Vector3 v = agent.destination;
            //// ã�� ���� ���� �� �ִ� ������ �ִ��� üũ�غ���.
            //for (int i = 0;  i < colliders.Length; i++)
            //{
            //    // �ּ��� target to spot �������� �߰��� �ɸ��� ���� ������Ѵ�.
            //    // BestSpot�� ã�ƾ���.
            //    Cover cover = colliders[i].GetComponent<Cover>();           // ���⼭ ���� ������ ���ؾ���.
            //    //int size = cover.GetCoverSpots().Length;

            //    // ���� �Ҵ� Spot�� ��� ���� �� �ִ� ������ �׻� �ٸ��� ������
            //    // �Ҵ� �����൵ �� ��?
            //    spots = cover.GetCoverSpots();
            //    // ���� ������ Ȯ�������� �� �ȿ� �����͸� ����.
            //    for(int j = 0; j < spots.Length; j++)
            //    {
            //        if (CheckIfSpotIsValid(spots[i]))
            //        {
            //            // sorting ������?
            //            //bestSpots.Add(System.Tuple.Create(spots[i], OriginToSpotbyCost(spots[i])));
            //            agent.SetDestination(spots[i].position);                        // ����� ��ǥ�� ��� �ڽ�Ʈ ���
            //            float distance = agent.remainingDistance;                       // �ڽ�Ʈ ��ȯ 
            //            spotcosts.Add(spots[j], distance);
            //        }
            //    }
            //}

            //List<float> valueList = spotcosts.Values.ToList();
            //// ������ dictionary ������ ���� ���� ģ���� ����
            ////foreach(var data in keyList)
            ////{

            ////}
            //valueList.Sort();
            //valueList.ToArray();
            //foreach(var val in valueList)
            //{

            //}
            //agent.SetDestination(spotcosts.ContainsValue(valueList.Find );

            #endregion



            #region legarcy

            float minAngle = 90;
            Transform bestSpot = null;
            for (int i = 0; i < avaliableCovers.Length; i++)
            {
                Transform bestSpotInCover = FindBestSpotInCover(avaliableCovers[i], ref minAngle);
                if (bestSpotInCover != null)
                    bestSpot = bestSpotInCover;
            }

            return bestSpot;

            #endregion
        }
        

        private Transform FindBestSpotInCover(Cover cover, ref float minAngle)
        {
            Transform bestSpot = null;
            try
            {
                Transform[] avaliableSpots = cover.GetCoverSpots();
                for (int i = 0; i < avaliableSpots.Length; i++)
                {
                    Vector3 direction = ai.NowTarget.position - avaliableSpots[i].position;
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
                Debug.Log("AA");
            }
            return bestSpot;
        }

        // ���� ���⿡ ���� ���� ���.
        public bool CheckIfPlayerToWay(Transform spot)
        {
            RaycastHit hit;
            Vector3 direction = ai.transform.forward;
            
            if(Physics.Raycast(spot .position, direction, out hit))
            {
                if(hit.collider.transform != ai.NowTarget)
                    return true;
            }

            return false;
        }


        private bool CheckIfSpotIsValid(Transform spot)
        {
            RaycastHit hit;

            Vector3 direction = ai.NowTarget.position - spot.position;

            if(Physics.Raycast(spot.position, direction, out hit))
            {
                if(hit.collider.transform != ai.NowTarget)
                { 
                    return true; 
                }
            }
            return false;
        }

    }
}
