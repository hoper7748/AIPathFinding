using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;


/// <summary>
/// 1. 기본적인 FPS기능은 구현이 되어 있음
/// 2. 그럼 내가 만들어야 할 내용은 무엇일까?
///     2-1. 개인전의 AI 
///     2-2. 팀전의 AI
///     2-3. 점령전의 AI
///     2-4. 시야각을 통한 서치 기능.
///     2-5. Navmesh를 직접 제작한 AiPathfinding으로 변경.
/// 3. 그럼 Node들을 분류해 보자.
///     3-0 기본으로 사용하게 될 노드들
///         3-0-1. 생각나는 상태 - 경계(대기), 탐색, 공격, 도주, 은닉, 추격, 기습, 사망
///         3-0-2. 은닉 시 추가 기능으로 앉는 기능(ypos - 0.5 같은거)을 추가해 숨음.
///         3-0-3. 상대가 나를 인지하지 못한 상태일 때 안정 사격 범위까지 들어가 사격.
///         3-0-4. Idle 상태에서 목표 지점으로 이동 시 Player가 바라보는 방향 기준으로 코너가 나오면 코너 직전에서 멈추고 코너를 잠깐 주시해야함. 양 옆일 경우 빠르게 좌 우를 확인함.
///     3-1 개인전
///         3-1-1. 개인전의 경우 필드에 남아있는 적의 스톡을 Maanger에서 관리하고 모든 적을 죽이면 게임이 끝나도록 설정해야 함.
///         3-1-2. 탐색의 경우 랜덤을 지향하지만, 적을 찾고 추격 중에 놓치는 경우 바라보는 방향으로 재탐색이 진행될 예정.
///         3-1-3. 
///         3-1-4.
///     3-2 팀전
///         3-2-1. 팀전의 경우 아군 적군 구분이 필요하면 아군의 경우 오인사격을 감안하여 분대 플레이, 포지셔닝을 할 수 있게 구현할 예정.
///         3-2-2. 팀전이니만큼 분대 플레이가 될 수 있도록 알고리즘을 짤 예정.
///         3-2-3. 최종적으로는 모든 적을 쓰러뜨리는 것이 목표이며, Manager에서 아군, 적군 수를 체크할 예정.
///         3-2-4. 적과 마주치는 경우 무전기가 있다는 가정 하에 아군에게 브리핑이 넘어가며, 지원을 요청하고 요청을 받은 아군은 아군이 있는 방향으로 재탐색을 시작함.
///         3-2-5. 
///         3-2-6. 
///     3-3 점령전
///         3-3-1. 점령전의 경우 지정된 위치로 이동하여 점령을 함. 섬멸이 목표가 아니기 때문에 죽은 적은 n초 후에 부활을 하고 다시 점령을 위해 움직임.
///         3-3-2. 점령 위치에 도착하면 점령 위치에서 경계를 함. 적이 오는 방향을 경계함.
///         3-3-3. 상대 팀이 점령하고 있는 중에 한 방향으로 가다가 여러번 죽으면(1 ~ 2 Death), 다른 입구에서 공격을 시도함.
///         3-3-4. 
///         3-3-5. 
///         
/// 
/// 
/// </summary>

public class EnemyAI : MonoBehaviour
{
    /// <summary>
    /// Field
    /// </summary>
    #region Fields
    // Status
    [SerializeField] private float startingHealth;
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] private float healthRestoreRate;

    [SerializeField] private float chasingRange;
    [SerializeField] private float shootingRange;

    [SerializeField] private Transform playertransform;
    [SerializeField] private Transform targetTransform;
    // covers
    [SerializeField] private GameObject covers;
    [SerializeField] private BT.Cover[] avaliableCovers;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask lay;

    private Material material;
    [SerializeField] private Transform bestCoverSpot;
    private NavMeshAgent agent;

    private BT.Node topNode;
    [SerializeField] private Bullet bullet;             // 공격 시 필요로 하게 되는 탄환 아직 미설정

    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject WayPoint;
    [HideInInspector] public Transform[] WayPoints;
    [HideInInspector] public Tuple<Vector3> movingPoint;

    private const float HorizontalViewAngle = 75;
    private float m_horizontalViewHalfAngle = 0f;
    private float rotateAngle = 0;
    [SerializeField] private float m_viewRotateZ = 0f;

    private bool isDead = false;

    #endregion

    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    public bool lostTarget = false;
    public bool isRun = false;
    public bool isHide = false;
    public GameObject target;
    public Transform shootPos;
    public Bullet[] bulletPool;
    public bool superAmmor;
    // 잃었다는 것은 RangeNode에서 찾아야하는데... 어떻게 찾아야 할까?
    // 잃었다 어떻게 확인할 것인가? 

    public Transform NowTarget
    {
        get
        {
            return targetTransform;
        }
        set
        {
            targetTransform = value;
        }
    }

    public float getCurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, startingHealth); }
    }

    #endregion

    #region DrawGizmos

    public static Vector3 AngleToDirY(Transform _transform, float angleInDegree)
    {
        #region Omit
        float radian = (angleInDegree + _transform.eulerAngles.y) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
        #endregion
    }

    public GameObject FindViewTarget(float SearchRange, LayerMask layer)
    {
        Vector3 targetPos, dir, lookDir;
        Vector3 originPos = transform.position;
        Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, layerMask);
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
            bool hitWall = Physics.Raycast(originPos, dir, Vector3.Distance(originPos, targetPos), layer);
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
                //return hitedTarget.gameObject;
            }
            //else if (Physics.Raycast(originPos, dir, out hit, layer))
            //{
            //    Debug.Log($"hit >> {hit.transform.name}");
            //}
        }
        // 걸린 애들 중에 가장 가까운 애들을 출력
        // 걸리는게 없나요? 정상입니다.

        return faraway == null ? null : faraway.gameObject;
    }


    private void OnDrawGizmos()
    {
        m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

        Vector3 originPos = transform.position - (transform.forward * 1.5f);

        //Gizmos.DrawWireSphere(transform.position, 2);

        Vector3 horizontalLeftDir = AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
        Vector3 horizontalRightDir = AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
        //Vector3 lookDir = AngleToDirY(transform, m_viewRotateZ);

        Debug.DrawRay(originPos, horizontalRightDir * chasingRange, Color.cyan);
        //Debug.DrawRay(originPos, lookDir * chasingRange, Color.green);
        Debug.DrawRay(originPos, horizontalLeftDir * chasingRange, Color.cyan);

        Debug.DrawLine(transform.position + transform.forward, transform.position + transform.forward - transform.right, Color.red);
        Debug.DrawLine(transform.position + transform.forward, transform.position + transform.forward + transform.right, Color.blue);
        if(target != null)
        {
            Gizmos.DrawWireSphere(target.transform.position ,1f);
        }
    }
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;
        //Time.timeScale = 2f;
    }

    private void Start()
    {
        // cover 갯수만큼 재생성
        isDead = false;
        covers = GameObject.Find("Covers");
        WayPoint = GameObject.Find("WayPoints");
        GetTransformChildren(ref avaliableCovers, covers.transform);
        GetTransformChildren(ref WayPoints, WayPoint.transform);
        getCurrentHealth = startingHealth;
        ConstructBehaviorTree();
    }


    private void GetTransformChildren<T>(ref T[] Container, Transform getChild)
    {
        Container = new T[getChild.childCount];
        for(int i =0; i < getChild.childCount; i++)
        {
            Container[i] = getChild.GetChild(i).GetComponent<T>();
        }
    }

    private void ConstructBehaviorTree()
    {
        // 기본 개인전의 경우.
        SoloModeBehaivorTree();
        
        // 팀전의 경우

        // 점령전의 경우

    }

    private void SoloModeBehaivorTree()
    {
        BT.IsCovereAvaliableNode coverAvaliableNode = new BT.IsCovereAvaliableNode(avaliableCovers, playertransform, this, agent, $"coverAvaliableNode");
        BT.GoToCoverNode goToCoverNode = new BT.GoToCoverNode(agent, this, $"goToCoverNode");
        BT.HealthNode healthNode = new BT.HealthNode(this, lowHealthThreshold, $"healthNode");
        BT.IsCoveredNode isCoveredNode = new BT.IsCoveredNode(playertransform, this, $"isCoveredNode");
        BT.ChaseNode chaseNode = new BT.ChaseNode(agent, this, $"chaseNode");

        BT.GoToDestinationNode goToDestinationNode = new BT.GoToDestinationNode(this, agent);
        BT.IsDestinationNode isDestinationNode = new BT.IsDestinationNode(this, agent);
        BT.IsHideObjectNode isHideObjectNode = new BT.IsHideObjectNode(this, agent);

        BT.SearchNode chasingRangeNode = new BT.SearchNode(chasingRange, this, FindViewTarget, $"chasingRangeNode");
        BT.SearchNode shootingRangeNode = new BT.SearchNode(shootingRange, this, FindViewTarget, $"shootingRangeNode");

        BT.ShootNode shootNode = new BT.ShootNode(agent, this, bullet, shootPos, $"ShootNode");
        BT.HasTargetNode hasTargetNode = new BT.HasTargetNode(this);

        // 비전투 관련 
        BT.IdleNode idleNode = new BT.IdleNode(this, agent);

        // 매복 관련 Nodes
        BT.AmbushNode ambushNode = new BT.AmbushNode(this);
        BT.IsHideNode isHideNode = new BT.IsHideNode(this);

        // 도망가려면 가라!
        BT.IsRunNode isRunNode = new BT.IsRunNode(this, "");

        // Boundary는 Idle 상태일 때 주변을 둘러보는 노드.
        BT.BoundaryNode boundaryNode = new BT.BoundaryNode(this, agent); // 아직 적용 안됨

        BT.IsAliveNode isAliveNode = new BT.IsAliveNode(this);
        BT.DeadNode deadNode = new BT.DeadNode(this);

        // Behaivor Tree

        // NonBattleSquence
        BT.Sequence goToDestinationSequence = new BT.Sequence(new List<BT.Node> { isDestinationNode, goToDestinationNode }, $"goToDestinationSequence");
        BT.Sequence IdleTestSequence = new BT.Sequence(new List<BT.Node> { idleNode }, $"IdleTestSequence");

        // Battle Sequence 
        BT.Sequence chaseSequence = new BT.Sequence(new List<BT.Node> { chasingRangeNode, chaseNode }, $"chaseSequence");
        BT.Sequence shootSequence = new BT.Sequence(new List<BT.Node> { shootingRangeNode, shootNode }, $"shootSequence");

        BT.Selector nonBattleSelector = new BT.Selector(new List<BT.Node> { goToDestinationSequence, IdleTestSequence }, $"nonCombatSequence");
        BT.Selector battleSelector = new BT.Selector(new List<BT.Node> { shootSequence, chaseSequence }, $"findEnemySequence");

        // NonBattle Nodes
        BT.Sequence goToCoverSequence = new BT.Sequence(new List<BT.Node> { coverAvaliableNode, goToCoverNode }, $"goToCoverSequence");
        BT.Selector findCoverSelector = new BT.Selector(new List<BT.Node> { goToCoverSequence, chaseSequence }, $"findCoverSelector");
        BT.Selector tryToTakeCoverSelector = new BT.Selector(new List<BT.Node> { isCoveredNode, findCoverSelector }, $"tryToTakeCoverSelector");
        BT.Sequence mainCoverSequence = new BT.Sequence(new List<BT.Node> { healthNode, tryToTakeCoverSelector }, $"mainCoverSequence");

        // 제 3자의 공격이 인식되었을 떄 행동 패턴.
        BT.Sequence anotherAttackRunCoverSequence = new BT.Sequence(new List<BT.Node> {isRunNode, tryToTakeCoverSelector },"anotherAttackRunCoverSequence");

        // Alive Sequence
        BT.Sequence lifeCheckSequence = new BT.Sequence(new List<BT.Node> { isAliveNode, deadNode }, $"lifeCheckSequecne");

        // Hide Sequence
        // 숨었을 때 상대방이 내 눈앞에 나타나면 공격을, 아니면 대기를 하고 대기를 하는 도중 공격을 받으면 hide 상태를 풀고 재차 도망을 간다.
        BT.Sequence surpriseAttackSequence = new BT.Sequence(new List<BT.Node> { ambushNode }, "surprise Attack Nodes");
        BT.Sequence hideSequence = new BT.Sequence(new List<BT.Node> { isHideNode, shootSequence }, $"hideSequence");
       
        topNode = new BT.Selector(new List<BT.Node> { lifeCheckSequence, hideSequence, mainCoverSequence, anotherAttackRunCoverSequence, battleSelector, nonBattleSelector }, $"topNode");
    }

    public float GetCurrentHealth()
    {
        return getCurrentHealth;
    }

    private void Update()
    {
        if(!isDead)
        {
            topNode.Evaluate();

            if (topNode.getNodeState == BT.NodeState.Failure)
            {
                SetColor(Color.red);
            }
            getCurrentHealth += Time.deltaTime * healthRestoreRate;
            if(UnderAttack())
            {
                // 공격을 받으면 회피
                //Debug.Log("헉! 누가 공격 하는거야?!");
                // 공격을 받으면 도망가야한다.
                isRun = true;
                isHide = false;
            }
        }
        else
        {
            SetColor(Color.black);
        }
    }


    // 공격을 받았는지 체크
    public bool UnderAttack()
    {
        Collider[] bullets = Physics.OverlapSphere(transform.position, 2f, 1 << 10);
        //Debug.Log($"bullets Count {bullets.Length}");
        foreach(var bullet in bullets)
        {
            if(bullet.GetComponent<Bullet>())
            {
                if(NowTarget == null)
                {
                    // Oner Forward Vector가 this.gameobject의 forward Vector와 직교하는지 체크.

                    NowTarget = bullet.GetComponent<Bullet>().getOner;
                    return false;
                }
                if (bullet.GetComponent<Bullet>().getOner != NowTarget && bullet.GetComponent<Bullet>().getOner != this.transform)
                {
                    Debug.Log($"{bullet.GetComponent<Bullet>().getOner.name} ? {NowTarget.name}");
                    // 다른 플레이어에게 공격을 받았다고 간주.
                    return true;
                }
            }
        }
        return false;
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

    public void SetBestCoverSopt(Transform _bestCoverSpot)
    {
        bestCoverSpot = _bestCoverSpot;
    }

    public Transform GetBestCoverSpot()
    {
        return bestCoverSpot;
    }

    public static Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time, float height = 1.5f)
    {
        #region Omit

        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
        distanceXZ.y = 0f; // y는 0으로 설정.
        float Sy = distance.y;    // 세로 높이의 거리를 지정.
        float Sxz = distanceXZ.magnitude;

        // 속도 추가
        float Vxz = Sxz / time;
        float Vy = Sy / time + height * Mathf.Abs(Physics.gravity.y) * time;
        // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
        #endregion
    }

    public void GetDamage(Transform shootOner)
    {
        // 데미지를 입었다! 그럼 내가 공격을 받고 있다는 뜻인데... 그럼 반격을 하든 공격을 하든 해야겠지?
        if (!superAmmor)
        {
            // hide인 상태에서 적에게 피해를 입으면? 
            currentHealth -= 10f;
            isHide = false;   // 바로 도망쳐
        }
        else
        {

        }
    }

    public void DeadPlayer()
    {
        isDead = true;
        Destroy(this.gameObject, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }


    public async UniTaskVoid LostTarget()
    {

        await UniTask.Delay(TimeSpan.FromSeconds(3f));
        // 내가 뭘 하려고 했지?
        NowTarget = null;
    }

    // 총성이 들리면 총성이 들린 방향을 향해 이동하는 노드. 
    // 매니저를 통해 전달을 받으며, 발사한 AI의 반경 n미터 내에 있는 AI들이 듣고 해당 위치로 찾아간다.
    // 발소리는 n -. 10 3?
    // public 
}
