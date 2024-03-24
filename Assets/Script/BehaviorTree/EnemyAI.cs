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
        BT.IsCovereAvaliableNode coverAvaliableNode = new BT.IsCovereAvaliableNode(avaliableCovers, playertransform, this, $"coverAvaliableNode");
        BT.GoToCoverNode goToCoverNode = new BT.GoToCoverNode(agent, this, $"goToCoverNode");
        BT.HealthNode healthNode = new BT.HealthNode(this, lowHealthThreshold, $"healthNode");
        BT.IsCoveredNode isCoveredNode = new BT.IsCoveredNode(playertransform, transform, $"isCoveredNode");
        BT.ChaseNode chaseNode = new BT.ChaseNode(playertransform, agent, this, "chaseNode");
        BT.RangeNode chasingRangeNode = new BT.RangeNode(chasingRange, playertransform, transform, $"chasingRangeNode");
        BT.RangeNode shootingRangeNode = new BT.RangeNode(shootingRange, playertransform, transform, $"shootingRangeNode");
        BT.ShootNode shootNode = new BT.ShootNode(agent, this, $"ShootNode");

        BT.Sequence chaseSequence = new BT.Sequence(new List<BT.Node> { chasingRangeNode, chaseNode }, "chaseSequence");
        BT.Sequence shootSequence = new BT.Sequence(new List<BT.Node> { shootingRangeNode, shootNode }, "shootSequence");

        BT.Sequence goToCoverSequence = new BT.Sequence(new List<BT.Node> { coverAvaliableNode, goToCoverNode }, "goToCoverSequence");
        BT.Selector findCoverSelector = new BT.Selector(new List<BT.Node> { goToCoverSequence, chaseSequence }, "findCoverSelector");
        BT.Selector tryToTakeCoverSelector = new BT.Selector(new List<BT.Node> { isCoveredNode, findCoverSelector }, "tryToTakeCoverSelector");
        BT.Sequence mainCoverSequence = new BT.Sequence(new List<BT.Node> { healthNode, tryToTakeCoverSelector }, "mainCoverSequence");

        topNode = new BT.Selector(new List<BT.Node> { mainCoverSequence, shootSequence, chaseSequence }, "topNode");
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
