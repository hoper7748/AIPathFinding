using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace BT
{
    public class IsCovereAvaliableNode : Node
    {
        // 여기서 BestCoverSpot을 찾는다.

        
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
            // 조건을 이렇게 찾으면 안됨!!!!
            // 현재 발각이 됐는가 부터 시작하여 다양한 경우의 수를 둬야함.

            // 현재 바라보는 방향에 적이 있는가?
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
                // 가장 먼 곳으로 가야함.
                if(faraway == null)
                {
                    faraway = cols[i].transform;
                    distanceOld = Vector3.Distance(ai.transform.position, faraway.position);
                }
                else
                {
                    // 거리를 비교해서 가장 먼 곳에 간다.
                    distanceOld = Vector3.Distance(ai.transform.position, faraway.position);
                    distanceNew = Vector3.Distance(ai.transform.position, cols[i].transform.position);
                    if(distanceNew >= distanceOld)
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
                faraway = FindBestSpotInCover(faraway.GetComponent<Cover>(), ref minAngle);
            }

            //GameObject obj = new GameObject();
            //obj.transform.position = faraway == null ? Vector3.zero : faraway.position;
            return faraway;
            
        }

        private Transform FindBestCoverSpot()
        {
            // 새로운 경로 찾기
            // 이미 도달해야할 목표가 있다?
            if(ai.GetBestCoverSpot() != null)
            {
                // 목표 지점으로 가는 길에 상대방이 존재.
                if(ai.FindViewTarget(30f, 1 << 9) == null)
                    return ai.GetBestCoverSpot();
            }
            // Spot찾는 알고리즘을 변경
            // 객체를 중심으로 원 범위를 탐색하여 찾은 장소들에 코스트를 매기고 최종적으로 가장 가까운 곳을 선택하여 출발하게 함.

            #region TestCode
            ////LayerMask a = 1 << 9;
            //Collider[] colliders = Physics.OverlapSphere(ai.transform.position, 30f, 1 << 9);
            ////agent.destination 
            //// 목표지점, 코스트
            ////List<System.Tuple<Transform, float>> bestSpots = new List<System.Tuple<Transform, float>>();
            //Dictionary<Transform, float> spotcosts = new Dictionary<Transform, float>();
            //Transform[] spots = null;
            //Vector3 v = agent.destination;
            //// 찾은 벽에 숨을 수 있는 공간이 있는지 체크해보자.
            //for (int i = 0;  i < colliders.Length; i++)
            //{
            //    // 최소한 target to spot 기준으로 중간에 걸리는 것이 없어야한다.
            //    // BestSpot을 찾아야함.
            //    Cover cover = colliders[i].GetComponent<Cover>();           // 여기서 이제 스폿을 비교해야함.
            //    //int size = cover.GetCoverSpots().Length;

            //    // 동적 할당 Spot의 경우 숨을 수 있는 공간이 항상 다르기 때문에
            //    // 할당 안해줘도 될 듯?
            //    spots = cover.GetCoverSpots();
            //    // 스폿 공간을 확보했으니 이 안에 데이터를 넣음.
            //    for(int j = 0; j < spots.Length; j++)
            //    {
            //        if (CheckIfSpotIsValid(spots[i]))
            //        {
            //            // sorting 어케함?
            //            //bestSpots.Add(System.Tuple.Create(spots[i], OriginToSpotbyCost(spots[i])));
            //            agent.SetDestination(spots[i].position);                        // 여기로 좌표로 잡고 코스트 계산
            //            float distance = agent.remainingDistance;                       // 코스트 반환 
            //            spotcosts.Add(spots[j], distance);
            //        }
            //    }
            //}

            //List<float> valueList = spotcosts.Values.ToList();
            //// 설정된 dictionary 내부의 가장 작은 친구를 반출
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

        // 가는 방향에 적이 있을 경우.
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
