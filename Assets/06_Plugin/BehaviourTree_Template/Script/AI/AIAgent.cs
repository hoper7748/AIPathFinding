using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

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
        [SerializeField] private float m_viewRotateZ = 0f;

        // PathFinding
        public pathFinding.Grid grid;
        public Vector3[] path;
        public Vector3 getLastPath
        {
            get
            {
                return path[path.Length - 1];
            }
        }
        public bool chasing = false;
        public bool finding = false;

        float speed = 5;
        int targetIndex;

        Action<Vector3[], bool> findNewPathFunc;

        #region Properties

        public GameObject target;
        [HideInInspector] public Vector3 LookPosition;
        [HideInInspector] public Vector3 TargetPosition;

        // �ǰ� ����
        [HideInInspector] public Vector3 hitDirection;

        [HideInInspector] public Transform bestCoverSpot;

        public Vector3 getPathPosition
        {
            get
            {
                return new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            }
        }

        public Transform GetBestCoverSpot()
        {
            return bestCoverSpot;
        }

        public void SetBestCoverSopt(Transform _bestCoverSpot)
        {
            bestCoverSpot = _bestCoverSpot;
        }
        #endregion 



        #region Find View Targets Func & Draw Gizmos

        public static Vector3 AngleToDirY(Transform _transform, float angleInDegree)
        {
            #region Omit
            float radian = (angleInDegree + _transform.eulerAngles.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            #endregion
        }

        // Direction�� ���� �ݴ� ������ ��ġ�� ��߾� �ϴ�...
        public Vector3 FindViewTargetHitPoint(float SearchRange, float HorizontalViewAngle, LayerMask hideMask, LayerMask targetMask)
        {
            #region Omit
            RaycastHit hit = new RaycastHit();
            Vector3 targetPos, dir, lookDir;
            Vector3 originPos = transform.position;
            Vector3 offset = Vector3.zero;
            // �÷��̾� ��Ī
            Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, targetMask);
            float dot, angle;

            foreach (var hitedTarget in hitedTargets)
            {
                targetPos = hitedTarget.transform.position;
                dir = (targetPos - originPos).normalized;
                lookDir = AngleToDirY(this.transform, rotateAngle);

                dot = Vector3.Dot(lookDir, dir);
                angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                bool hitWall = Physics.Raycast(originPos, dir, out hit, Vector3.Distance(originPos, targetPos), hideMask);
                if (angle <= HorizontalViewAngle * .5f &&
                    hitWall)
                {
                    // ��ȯ�ؾ� �ϴ� �͵� -> 1. �ش� ��ġ�κ��� ���������� ����� �Ÿ�.
                    Vector3 direction = hit.transform.position - hit.point;
                    float distance = Vector3.Distance(hit.point, hit.transform.position);
                    //if (hit.transform.GetComponent<BT.Cover>() is BT.Cover cover)
                    //{
                    //    offset.x = cover.MeshSize.width - (cover.MeshSize.x - hit.point.x);
                    //    offset.z = cover.MeshSize.height - (cover.MeshSize.y - hit.point.z);
                    //}
                    //else
                    //{ 
                    offset.x = direction.normalized.x < 0 ? -3 : 3;
                    offset.z = direction.normalized.z < 0 ? -3 : 3;
                    //}
                    //Instantiate(new GameObject(), hit.transform.position + (direction) + offset, Quaternion.identity);
                    direction.y = transform.position.y;
                    return hit.transform.position + (direction  + offset);
                }
            }

            return target.transform == null ? Vector3.zero : target.transform.position;

            //return faraway == null ? null : faraway.gameObject;
            #endregion
        }

        public GameObject FindViewTarget(float SearchRange, float HorizontalViewAngle, LayerMask hideMask, LayerMask targetMask )
        {
            #region Omit
            RaycastHit hit;
            Vector3 targetPos, dir, lookDir;
            Vector3 originPos = transform.position;
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
                bool hitWall = Physics.Raycast(originPos, dir, out hit, Vector3.Distance(originPos, targetPos), hideMask);
                if (angle <= HorizontalViewAngle * .5f &&
                    !hitWall
                    && hitedTarget != this)
                {
                    if (faraway == null)
                    {
                        faraway = hitedTarget.transform;
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                    }
                    else
                    {
                        distanceOld = Vector3.Distance(transform.position, faraway.position);
                        distanceNew = Vector3.Distance(transform.position, hitedTarget.transform.position);
                        if (distanceNew <= distanceOld)
                        {
                            faraway = hitedTarget.transform;
                            distanceOld = distanceNew;
                        }
                    }
                } 
            }

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


        #region Magic Func
        private void Awake()
        {
            #region Omit
            // ĳ��.
            navMeshAgent = GetComponent<NavMeshAgent>();

            // Action �Լ� �߰�
            findNewPathFunc = OnPathFound;
            Gizmos.color = Color.white;
            
            #endregion 
        }

        // Start is called before the first frame update
        void Start()
        {
            // �̰� ��θ� �����ϴ� ����.
            // -> ���� ��ġ�� ��ǥ������ ���� �����ϰ� OnPathFound�� ���� ������ ��θ� Ž����.
            // -> �̰� ���ָ� Path�� ã�� ����ϰ� OnPathFound �Լ��� ����� �̵��� �����Ѵ�.
            //pathFinding.PathRequestManager.RequestPath(this.transform.position, target.transform.position, OnPathFound);

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

        #endregion

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

        // Ÿ�̸� 
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
        public void StopPathFinding()
        {
            targetIndex = 0;
            path = null;
            StopCoroutine("FollowPath");
        }

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

        IEnumerator FollowPath()
        {
            #region Omit
            int count = 0;
            Vector3 currentWaypoint = path[0];
            Vector3 dir = Vector3.zero;
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
                dir = currentWaypoint - transform.position;

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), speed * Time.deltaTime);
               
                yield return null;
            }
            #endregion
        }

        // ��ǥ���� �̵��� �����Ǹ� üũ�� ��.
        // Rect�� ĭ �� ������ �����ϰ� ������ ������ üũ�ϴ� ����
        public bool CheckTargetPosition()
        {
            #region Omit
            // Ÿ�� ���
            //Vector3 targetPos = grid.NodeFromWorldPoint(target.transform.position).worldPosition;
            Vector3 targetPos = grid.NodeFromWorldPoint(getLastPath).worldPosition;
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


        #endregion

    }

}