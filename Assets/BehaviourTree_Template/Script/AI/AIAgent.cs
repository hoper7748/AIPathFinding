using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    // ����
    // -> ��븦 ã���� �ٷ� ���� 
    // -> �׷��� ��뿡�� ������ �ް��ִ� ���¶�� �������.
    // -> ��� ����� �ұ�? �ֺ��� ���� ����� ����ó�� ã�ƾ���.
    //
    //
    public class AIAgent : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;

        public bool FindTarget = false;

        public float chasingRange;
        public float shootingRange;
        public bool shoot = false;

        // ���� �����ƴ�!!
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
        // �ǰ� ����
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
            // �÷��̾� ��Ī
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
                    // �þ� �� ���� ������ �Ÿ��� ��
                    // ���� �� ������ ������.
                    if (faraway == null)
                    {
                        faraway = hitedTarget.transform;
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                    }
                    else
                    {
                        // �Ÿ��� ���ؼ� ���� �� ���� ����.
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                        distanceNew = Vector3.Distance(transform.position, hitedTarget.transform.position);
                        if (distanceNew <= distanceOld)
                        {
                            // ���� ����� ���� ���� ����
                            faraway = hitedTarget.transform;
                            distanceOld = distanceNew;
                        }
                    }
                }
            }
            // �ɸ� �ֵ� �߿� ���� ����� �ֵ��� ���
            // �ɸ��°� ������? �����Դϴ�.

            return faraway == null ? null : faraway.gameObject;
            #endregion
        }


        private void OnDrawGizmos()
        {
            #region Omit
            m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position - (transform.forward * 1.5f);

            // �ǰ� ����
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
            // ĳ��.
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // �̰� ��θ� �����ϴ� ����.
            // -> ���� ��ġ�� ��ǥ������ ���� �����ϰ� OnPathFound�� ���� ������ ��θ� Ž����.
            pathFinding.PathRequestManager.RequestPath(this.transform.position, target.transform.position, OnPathFound);

        }



        private float SearchTimer = 0.5f;
        private float curTimer = 0;
        // Update is called once per frame
        void Update()
        {
            // ������ �ް� �����鼭 ���� ���� �����ΰ�?
            if (!isHit && UnderAttack() )
            {
                Debug.Log("AA");
            }
            if(target != null)
            {
                Debug.DrawLine(transform.position, target.transform.position);
            }
            
            // ������ ���� �ٶ󺸴°Ŷ� Dir�� ���� �ʿ䰡 ���� �׷� ��F�� dir�� üũ�� ���ΰ�?
            // hit point�� ��� transform class �ƴ� vector3 ����ü�� �̷���� �ֱ� ������ �⺻������ set�� �� ��� (0, 0, 0);
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
                    // �Ѿ��� ���ƿ��� �Ѿ��� ���ƿ� ������ �������� ���� �� �ִ� ���� ����� ������ ���´�.
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

        // ��ǥ���� �̵��� �����Ǹ� üũ�� ��.
        // Rect�� ĭ �� ������ �����ϰ� ������ ������ üũ�ϴ� ����
        public bool CheckTargetPosition()
        {
            #region Omit
            // Ÿ�� ���
            Vector3 targetPos = grid.NodeFromWorldPoint(target.transform.position).worldPosition;
            // ���� ��ǥ
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