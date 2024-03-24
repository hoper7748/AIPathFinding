using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] private float healthRestoreRate;

    [SerializeField] private float chasingRange;
    [SerializeField] private float shootingRange;

    [SerializeField] private Transform playertransform;
    [SerializeField] private BT.Cover[] avaliableCovers;

    private Material material;
    private Transform bestCoverSpot;
    private NavMeshAgent agent;

    private BT.Node topNode;


    private float currentHealth;

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
        BT.IsCovereAvaliableNode coverAvaliableNode = new BT.IsCovereAvaliableNode(avaliableCovers, playertransform, this);
        BT.GoToCoverNode goToCoverNode = new BT.GoToCoverNode(agent, this);
        BT.HealthNode healthNode = new BT.HealthNode(this, lowHealthThreshold);
        BT.IsCoveredNode isCoveredNode = new BT.IsCoveredNode(playertransform, transform);
        BT.ChaseNode chaseNode = new BT.ChaseNode(playertransform, agent, this);
        BT.RangeNode chasingRangeNode = new BT.RangeNode(shootingRange, playertransform, transform);
        BT.RangeNode shootingRangeNode = new BT.RangeNode(shootingRange, playertransform, transform);
        BT.ShootNode shootNode = new BT.ShootNode(agent, this);

        BT.Sequence chaseSequence = new BT.Sequence(new List<BT.Node> { chasingRangeNode, chaseNode });
        BT.Sequence shootSequence = new BT.Sequence(new List<BT.Node> { shootingRangeNode, shootNode });

        BT.Sequence goToCoverSequence = new BT.Sequence(new List<BT.Node> { coverAvaliableNode, goToCoverNode });
        BT.Selector findCoverSelector = new BT.Selector(new List<BT.Node> { goToCoverSequence, chaseSequence });
        BT.Selector tryToTakeCoverSelector = new BT.Selector(new List<BT.Node> { isCoveredNode, findCoverSelector });
        BT.Sequence mainCoverSequence = new BT.Sequence(new List<BT.Node> { healthNode, tryToTakeCoverSelector });

        topNode = new BT.Selector(new List<BT.Node> { mainCoverSequence, shootSequence, chaseSequence });
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
