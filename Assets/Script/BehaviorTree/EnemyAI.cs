using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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
    [SerializeField] private float startingHealth;
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] private float healthRestoreRate;

    [SerializeField] private float chasingRange;
    [SerializeField] private float shootingRange;

    [SerializeField] private Transform playertransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private BT.Cover[] avaliableCovers;
    [SerializeField] private LayerMask layerMask;

    private Material material;
    [SerializeField] private Transform bestCoverSpot;
    private NavMeshAgent agent;

    private BT.Node topNode;
    private GameObject bullet;             // 공격 시 필요로 하게 되는 탄환 아직 미설정

    private float currentHealth;
    [HideInInspector] public Vector3 _movingPoint;
    [HideInInspector] public Tuple<Vector3, bool> movingPoint;


    private const float HorizontalViewAngle = 60f;
    private float m_horizontalViewHalfAngle = 0f;
    private float rotateAngle = 0;
    [SerializeField] private float m_viewRotateZ = 0f;

    public Transform NowTarget
    {
        get
        {
            return targetTransform;
        }
        set
        {
            NowTarget = value;  
        }
    }

    #region DrawGizmos

    private Vector3 AngleToDirY(Transform _transform, float angleInDegree)
    {
        #region Omit
        float radian = (angleInDegree + _transform.eulerAngles.y) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
        #endregion
    }

    public GameObject FindViewTarget(float SearchRange)
    {
        Vector3 targetPos, dir, lookDir;
        Vector3 originPos = transform.position;
        Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, layerMask);

        float dot, angle;

        foreach (var hitedTarget in hitedTargets)
        {
            targetPos = hitedTarget.transform.position;
            dir = (targetPos - originPos).normalized;
            lookDir = AngleToDirY(this.transform, rotateAngle);

            dot = Vector3.Dot(lookDir, dir);
            angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle <= HorizontalViewAngle * .5f)
            {
                // 타겟이 걸리면 반환.
                return hitedTarget.gameObject;
            }
        }
        // 걸리는게 없나요? 정상입니다.
        return null;
    }


    private void OnDrawGizmos()
    {
        m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

        Vector3 originPos = transform.position;

        Gizmos.DrawWireSphere(originPos, chasingRange);

        Vector3 horizontalRightDir = AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
        Vector3 horizontalLeftDir = AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
        Vector3 lookDir = AngleToDirY(transform, m_viewRotateZ);

        Debug.DrawRay(originPos, horizontalLeftDir * chasingRange, Color.cyan);
        Debug.DrawRay(originPos, lookDir * chasingRange, Color.green);
        Debug.DrawRay(originPos, horizontalRightDir * chasingRange, Color.cyan);

    }
    #endregion

    private float getCurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, startingHealth); }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        getCurrentHealth = startingHealth;
        ConstructBehaviorTree();
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

        BT.IsCovereAvaliableNode coverAvaliableNode = new BT.IsCovereAvaliableNode(avaliableCovers, playertransform, this, $"coverAvaliableNode");
        BT.GoToCoverNode goToCoverNode = new BT.GoToCoverNode(agent, this, $"goToCoverNode");
        BT.HealthNode healthNode = new BT.HealthNode(this, lowHealthThreshold, $"healthNode");
        BT.IsCoveredNode isCoveredNode = new BT.IsCoveredNode(playertransform, transform, $"isCoveredNode");
        BT.ChaseNode chaseNode = new BT.ChaseNode(playertransform, agent, this, $"chaseNode");
        BT.SearchNode chasingRangeNode = new BT.SearchNode(chasingRange, playertransform, transform, FindViewTarget, $"chasingRangeNode");
        BT.SearchNode shootingRangeNode = new BT.SearchNode(shootingRange, playertransform, transform, FindViewTarget, $"shootingRangeNode");
        BT.ShootNode shootNode = new BT.ShootNode(agent, this, bullet, $"ShootNode");

        //BT.IsTargetNode 



        BT.IdleNode idleNode = new BT.IdleNode(this, agent);
        //BT.BoundaryNode Boundary = new BT.BoundaryNode(agent);


        BT.Sequence chaseSequence = new BT.Sequence(new List<BT.Node> { chasingRangeNode, chaseNode }, $"chaseSequence");
        BT.Sequence shootSequence = new BT.Sequence(new List<BT.Node> { shootingRangeNode, shootNode }, $"shootSequence");

        BT.Sequence nonCombatSequence = new BT.Sequence(new List<BT.Node> { idleNode }, $"idleSequenece");
        BT.Selector combatSequence = new BT.Selector(new List<BT.Node> { shootSequence, chaseSequence }, $"findEnemySequence");

        BT.Sequence goToCoverSequence = new BT.Sequence(new List<BT.Node> { coverAvaliableNode, goToCoverNode }, $"goToCoverSequence");
        BT.Selector findCoverSelector = new BT.Selector(new List<BT.Node> { goToCoverSequence, chaseSequence }, $"findCoverSelector");
        BT.Selector tryToTakeCoverSelector = new BT.Selector(new List<BT.Node> { isCoveredNode, findCoverSelector }, $"tryToTakeCoverSelector");
        BT.Sequence mainCoverSequence = new BT.Sequence(new List<BT.Node> { healthNode, tryToTakeCoverSelector }, $"mainCoverSequence");

        topNode = new BT.Selector(new List<BT.Node> { mainCoverSequence, combatSequence, nonCombatSequence }, $"topNode");
    }

    public float GetCurrentHealth()
    {
        return getCurrentHealth;
    }

    private void Update()
    {
        topNode.Evaluate();

        if(topNode.getNodeState == BT.NodeState.Failure)
        {
            SetColor(Color.red);
        }
        getCurrentHealth += Time.deltaTime * healthRestoreRate;
    }

    private void OnMouseDown()
    {
        currentHealth -= 10f;
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
}
