using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;


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
///         3-0-4. Idle ���¿��� ��ǥ �������� �̵� �� Player�� �ٶ󺸴� ���� �������� �ڳʰ� ������ �ڳ� �������� ���߰� �ڳʸ� ��� �ֽ��ؾ���. �� ���� ��� ������ �� �츦 Ȯ����.
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
    [SerializeField] private Bullet bullet;             // ���� �� �ʿ�� �ϰ� �Ǵ� źȯ ���� �̼���

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
    // �Ҿ��ٴ� ���� RangeNode���� ã�ƾ��ϴµ�... ��� ã�ƾ� �ұ�?
    // �Ҿ��� ��� Ȯ���� ���ΰ�? 

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
                //return hitedTarget.gameObject;
            }
            //else if (Physics.Raycast(originPos, dir, out hit, layer))
            //{
            //    Debug.Log($"hit >> {hit.transform.name}");
            //}
        }
        // �ɸ� �ֵ� �߿� ���� ����� �ֵ��� ���
        // �ɸ��°� ������? �����Դϴ�.

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
        // cover ������ŭ �����
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
        // �⺻ �������� ���.
        SoloModeBehaivorTree();
        
        // ������ ���

        // �������� ���

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

        // ������ ���� 
        BT.IdleNode idleNode = new BT.IdleNode(this, agent);

        // �ź� ���� Nodes
        BT.AmbushNode ambushNode = new BT.AmbushNode(this);
        BT.IsHideNode isHideNode = new BT.IsHideNode(this);

        // ���������� ����!
        BT.IsRunNode isRunNode = new BT.IsRunNode(this, "");

        // Boundary�� Idle ������ �� �ֺ��� �ѷ����� ���.
        BT.BoundaryNode boundaryNode = new BT.BoundaryNode(this, agent); // ���� ���� �ȵ�

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

        // �� 3���� ������ �νĵǾ��� �� �ൿ ����.
        BT.Sequence anotherAttackRunCoverSequence = new BT.Sequence(new List<BT.Node> {isRunNode, tryToTakeCoverSelector },"anotherAttackRunCoverSequence");

        // Alive Sequence
        BT.Sequence lifeCheckSequence = new BT.Sequence(new List<BT.Node> { isAliveNode, deadNode }, $"lifeCheckSequecne");

        // Hide Sequence
        // ������ �� ������ �� ���տ� ��Ÿ���� ������, �ƴϸ� ��⸦ �ϰ� ��⸦ �ϴ� ���� ������ ������ hide ���¸� Ǯ�� ���� ������ ����.
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
                // ������ ������ ȸ��
                //Debug.Log("��! ���� ���� �ϴ°ž�?!");
                // ������ ������ ���������Ѵ�.
                isRun = true;
                isHide = false;
            }
        }
        else
        {
            SetColor(Color.black);
        }
    }


    // ������ �޾Ҵ��� üũ
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
                    // Oner Forward Vector�� this.gameobject�� forward Vector�� �����ϴ��� üũ.

                    NowTarget = bullet.GetComponent<Bullet>().getOner;
                    return false;
                }
                if (bullet.GetComponent<Bullet>().getOner != NowTarget && bullet.GetComponent<Bullet>().getOner != this.transform)
                {
                    Debug.Log($"{bullet.GetComponent<Bullet>().getOner.name} ? {NowTarget.name}");
                    // �ٸ� �÷��̾�� ������ �޾Ҵٰ� ����.
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
        Vector3 distanceXZ = distance; // x�� z�� ����̸� �⺻������ �Ÿ��� ���� ����.
        distanceXZ.y = 0f; // y�� 0���� ����.
        float Sy = distance.y;    // ���� ������ �Ÿ��� ����.
        float Sxz = distanceXZ.magnitude;

        // �ӵ� �߰�
        float Vxz = Sxz / time;
        float Vy = Sy / time + height * Mathf.Abs(Physics.gravity.y) * time;
        // ������� ���� �� ���� �ʱ� �ӵ��� ������ ���ο� ���͸� ���� �� ����.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
        #endregion
    }

    public void GetDamage(Transform shootOner)
    {
        // �������� �Ծ���! �׷� ���� ������ �ް� �ִٴ� ���ε�... �׷� �ݰ��� �ϵ� ������ �ϵ� �ؾ߰���?
        if (!superAmmor)
        {
            // hide�� ���¿��� ������ ���ظ� ������? 
            currentHealth -= 10f;
            isHide = false;   // �ٷ� ������
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
        // ���� �� �Ϸ��� ����?
        NowTarget = null;
    }

    // �Ѽ��� �鸮�� �Ѽ��� �鸰 ������ ���� �̵��ϴ� ���. 
    // �Ŵ����� ���� ������ ������, �߻��� AI�� �ݰ� n���� ���� �ִ� AI���� ��� �ش� ��ġ�� ã�ư���.
    // �߼Ҹ��� n -. 10 3?
    // public 
}
