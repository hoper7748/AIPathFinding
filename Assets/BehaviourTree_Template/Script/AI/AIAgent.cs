using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    // 조건
    // -> 상대를 찾으면 바로 공격 
    // -> 그러나 상대에게 공격을 받고있는 상태라면 숨어야함.
    // -> 어떻게 숨어야 할까? 주변에 가장 가까운 은신처를 찾아야함.
    //
    //
    public class AIAgent : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;

        public bool FindTarget = false;

        public float chasingRange;
        public float shootingRange;
        public bool shoot = false;

        // 적과 마주쳤다!!
        public bool isEncounter = false;
        public bool isHit = false;
        public bool isConcealment = false;
        public bool isHide = false;

        private const float HorizontalViewAngle = 75;
        private float m_horizontalViewHalfAngle = 0f;
        private float rotateAngle = 0;

        public pathFinding.Grid grid;
        public Vector3[] path;
        public bool chasing = false;
        public bool finding = false;

        float speed = 1;
        int targetIndex;


        [SerializeField] private float m_viewRotateZ = 0f;

        public GameObject target;
        [HideInInspector] public Vector3 LookPosition;
        // 피격 방향
        [HideInInspector] public Vector3 hitDirection;

        [HideInInspector] public Transform bestCoverSpot;

        #region Find View Targets

        public static Vector3 AngleToDirY(Transform _transform, float angleInDegree)
        {
            #region Omit
            float radian = (angleInDegree + _transform.eulerAngles.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            #endregion
        }

        public GameObject FindViewTarget(float SearchRange, LayerMask hideMask, LayerMask targetMask )
        {
            #region Omit
            Vector3 targetPos, dir, lookDir;
            Vector3 originPos = transform.position;
            // 플레이어 서칭
            Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, targetMask);
            Transform faraway = null;
            float dot, angle;
            float distanceOld, distanceNew;

            foreach (var hitedTarget in hitedTargets)
            {
                targetPos = hitedTarget.transform.position;
                dir = (targetPos - originPos).normalized;
                lookDir = AngleToDirY(this.transform, rotateAngle);

                dot = Vector3.Dot(lookDir, dir);
                angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                bool hitWall = Physics.Raycast(originPos, dir, Vector3.Distance(originPos, targetPos), hideMask);
                //Debug.Log($"aa = {a}");
                if (angle <= HorizontalViewAngle * .5f &&
                    !hitWall
                    && hitedTarget != this)
                {
                    // 시야 각 내에 있으니 거리를 비교
                    // 가장 먼 곳으로 가야함.
                    if (faraway == null)
                    {
                        faraway = hitedTarget.transform;
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                    }
                    else
                    {
                        // 거리를 비교해서 가장 먼 곳에 간다.
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                        distanceNew = Vector3.Distance(transform.position, hitedTarget.transform.position);
                        if (distanceNew <= distanceOld)
                        {
                            // 가장 가까운 적을 향해 발포
                            faraway = hitedTarget.transform;
                            distanceOld = distanceNew;
                        }
                    }
                }
            }
            // 걸린 애들 중에 가장 가까운 애들을 출력
            // 걸리는게 없나요? 정상입니다.

            return faraway == null ? null : faraway.gameObject;
            #endregion
        }


        private void OnDrawGizmos()
        {
            #region Omit
            m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position - (transform.forward * 1.5f);

            // 피격 범위
            Gizmos.DrawWireSphere(transform.position, 2);

            Vector3 horizontalLeftDir = AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalRightDir = AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
            //Vector3 lookDir = AngleToDirY(transform, m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalRightDir * chasingRange, Color.cyan);
            //Debug.DrawRay(originPos, lookDir * chasingRange, Color.green);
            Debug.DrawRay(originPos, horizontalLeftDir * chasingRange, Color.cyan);

            Debug.DrawLine(transform.position + transform.forward, transform.position + transform.forward - transform.right, Color.red);
            Debug.DrawLine(transform.position + transform.forward, transform.position + transform.forward + transform.right, Color.blue);
            Vector3 cur = Vector3.zero;
            Vector3 prev = Vector3.zero;
            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    cur.Set(path[i].x, path[i].y + 1f, path[i].z);
                    
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(cur, Vector3.one);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, cur);
                    }
                    else
                    {
                        prev.Set(path[i - 1].x, path[i - 1].y, path[i - 1].z);
                        Gizmos.DrawLine(prev, cur);
                    }
                }
            }
            #endregion
        }
        #endregion

        private void Awake()
        {
            // 캐싱.
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // 이게 경로를 세팅하는 것임.
            // -> 현재 위치와 목표지점을 먼저 지정하고 OnPathFound를 통해 최적의 경로를 탐색함.
            pathFinding.PathRequestManager.RequestPath(this.transform.position, target.transform.position, OnPathFound);

        }



        private float SearchTimer = 0.5f;
        private float curTimer = 0;
        // Update is called once per frame
        void Update()
        {
            // 위협을 받고 있으면서 교전 중인 상태인가?
            if (!isHit && UnderAttack() )
            {
                Debug.Log("AA");
            }
            if(target != null)
            {
                Debug.DrawLine(transform.position, target.transform.position);
            }
            
            // 방향을 향해 바라보는거라 Dir을 구할 필요가 있음 그럼 어덯게 dir을 체크할 것인가?
            // hit point의 경우 transform class 아닌 vector3 구조체로 이루어져 있기 때문에 기본적으로 set이 될 경우 (0, 0, 0);
            //if(true)
                //transform.forward = Vector3.Lerp(transform.forward, dir, 5 * Time.deltaTime);
        }

        public bool UnderAttack()
        {
            #region Omit
            Collider[] bullets = Physics.OverlapSphere(transform.position, 2f, 1 << 10);
            //Debug.Log($"bullets Count {bullets.Length}");
            foreach (var bullet in bullets)
            {
                Bullet bullet1 = bullet.GetComponent<Bullet>();
                if (bullet1)
                {
                    // 총알이 날아오면 총알이 날아온 방향을 기준으로 숨을 수 있는 가장 가까운 공간에 숨는다.
                    isHit = true;
                    target = bullet1.getOner.gameObject;
                    return true;
                }
            }
            return false;
            #endregion
        }

        private bool CheckTimer()
        {
            #region Omit
            curTimer += Time.deltaTime;
            if(curTimer >= SearchTimer)
            {
                shoot = false;
                curTimer = 0;
                return true;
            }
            return false;
            #endregion
        }


        // PathFinding
        #region PathFinding
        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            #region Omit
            if (pathSuccessful)
            {
                chasing = true;
                targetIndex = 0;
                path = newPath;
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
            #endregion
        }

        // 목표물의 이동이 감지되면 체크를 함.
        // Rect로 칸 당 범위를 생성하고 생성된 범위를 체크하는 형식
        public bool CheckTargetPosition()
        {
            #region Omit
            // 타겟 노드
            Vector3 targetPos = grid.NodeFromWorldPoint(target.transform.position).worldPosition;
            // 최종 목표
            Vector3 FinTarget = path[path.Length - 1];

            float distance = Mathf.Abs(Vector3.Distance(targetPos, FinTarget));

            // 1.5 * n
            if (distance > 1.5f * 4)
            {
                return true;
            }

            return false;
            #endregion
        }

        IEnumerator FollowPath()
        {
            #region Omit
            int count = 0;
            Vector3 currentWaypoint = path[0];
            while (true)
            {
                count++;
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        finding = true;
                        chasing = false;
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                if (count > 100000)
                {
                    transform.GetComponent<AIAgent>().enabled = false;
                    yield break;
                }
                if (CheckTargetPosition())
                {
                    chasing = false;
                    break;
                }

                currentWaypoint.y = transform.position.y;

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
            #endregion
        }

        #endregion


        #region Properties
        public Transform GetBestCoverSpot()
        {
            return bestCoverSpot;
        }

        public void SetBestCoverSopt(Transform _bestCoverSpot)
        {
            bestCoverSpot = _bestCoverSpot;
        }
        #endregion 

    }

}