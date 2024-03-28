using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 1. �⺻���� FPS����� ������ �Ǿ� ����
/// 2. �׷� ���� ������ �� ������ �����ϱ�?
///     2-1. �������� AI 
///     2-2. ������ AI
///     2-3. �������� AI
///     2-4. �þ߰��� ���� ��ġ ���.
///     2-5. Navmesh�� ���� ������ AiPathfinding���� ����.
/// 3. �׷� Node���� �з��� ����.
///     3-0 �⺻���� ����ϰ� �� ����
///         3-0-1. �������� ���� - ���(���), Ž��, ����, ����, ����, �߰�, ���, ���
///         3-0-2. ���� �� �߰� ������� �ɴ� ���(ypos - 0.5 ������)�� �߰��� ����.
///         3-0-3. ��밡 ���� �������� ���� ������ �� ���� ��� �������� �� ���.
///     3-1 ������
///         3-1-1. �������� ��� �ʵ忡 �����ִ� ���� ������ Maanger���� �����ϰ� ��� ���� ���̸� ������ �������� �����ؾ� ��.
///         3-1-2. Ž���� ��� ������ ����������, ���� ã�� �߰� �߿� ��ġ�� ��� �ٶ󺸴� �������� ��Ž���� ����� ����.
///         3-1-3. 
///         3-1-4.
///     3-2 ����
///         3-2-1. ������ ��� �Ʊ� ���� ������ �ʿ��ϸ� �Ʊ��� ��� ���λ���� �����Ͽ� �д� �÷���, �����Ŵ��� �� �� �ְ� ������ ����.
///         3-2-2. �����̴ϸ�ŭ �д� �÷��̰� �� �� �ֵ��� �˰����� © ����.
///         3-2-3. ���������δ� ��� ���� �����߸��� ���� ��ǥ�̸�, Manager���� �Ʊ�, ���� ���� üũ�� ����.
///         3-2-4. ���� ����ġ�� ��� �����Ⱑ �ִٴ� ���� �Ͽ� �Ʊ����� �긮���� �Ѿ��, ������ ��û�ϰ� ��û�� ���� �Ʊ��� �Ʊ��� �ִ� �������� ��Ž���� ������.
///         3-2-5. 
///         3-2-6. 
///     3-3 ������
///         3-3-1. �������� ��� ������ ��ġ�� �̵��Ͽ� ������ ��. ������ ��ǥ�� �ƴϱ� ������ ���� ���� n�� �Ŀ� ��Ȱ�� �ϰ� �ٽ� ������ ���� ������.
///         3-3-2. ���� ��ġ�� �����ϸ� ���� ��ġ���� ��踦 ��. ���� ���� ������ �����.
///         3-3-3. ��� ���� �����ϰ� �ִ� �߿� �� �������� ���ٰ� ������ ������(1 ~ 2 Death), �ٸ� �Ա����� ������ �õ���.
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
    private GameObject bullet;             // ���� �� �ʿ�� �ϰ� �Ǵ� źȯ ���� �̼���

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
                // Ÿ���� �ɸ��� ��ȯ.
                return hitedTarget.gameObject;
            }
        }
        // �ɸ��°� ������? �����Դϴ�.
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
        // �⺻ �������� ���.
        SoloModeBehaivorTree();
        
        // ������ ���

        // �������� ���

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
