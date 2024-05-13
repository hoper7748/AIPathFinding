using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class WeightSelectorNode : CompositeNode
    {
        // 총 가중치
        [SerializeField] private float totalWeights = 0;
        private int childCurrent = 0;
        private Node currentNode;
       
        
        public WeightSelectorNode()
        {
            onChildAdded += OnChildAdded;
            onChildRemoved += OnChildRemoved;
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            currentNode = null;
        }

        protected override State OnUpdate()
        {
            // 가중치를 부여하고 부여된 가중치를 통해 
            if (children.Count == 0) return State.Failure;

            currentNode = GetStartingNode();
            if ( currentNode == null)
            {
                return State.Success;
            }
            return currentNode.Update();
        }

        public Node GetStartingNode()
        {
            if (currentNode != null && currentNode.state == State.Running) return currentNode;
            float rand = Random.Range(0, totalWeights);
            float sum = 0;
            foreach (Node child in children)
            {
                if (child is IWeightNode weightChild)
                {
                    float weight = weightChild.Weight;
                    sum += weight;

                    if (rand < sum)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        private void OnChildAdded(Node node)
        {

            if (node is WeightDecoratorNode weightDecorator)
            {
                totalWeights += weightDecorator.Weight;
                weightDecorator.onWeightChanged += OnWeightChanged;

                return;
            }

            //FDebug.LogWarning("Used Invalid Node. This Node's children should be Weight Node. Please Check Children of this Node", GetType());
        }
        private void OnChildRemoved(Node node)
        {


            if (node is WeightDecoratorNode weightDecorator)
            {
                totalWeights -= weightDecorator.Weight;
                weightDecorator.onWeightChanged -= OnWeightChanged;

                if (children.Count == 0) { totalWeights = 0; }
                return;
            }

            //FDebug.LogWarning("Used Invalid Node. This Node's children should be Weight Node. Please Check Children of this Node", GetType());
        }

        private void OnWeightChanged(float oldWeight, float newWeight)
        {
            totalWeights -= oldWeight;
            totalWeights += newWeight;
        }
        public override Node Clone()
        {
            var node = base.Clone() as WeightSelectorNode;

            node.totalWeights = 0;
            foreach (var child in node.children)
            {
                node.OnChildAdded(child);
            }

            return node;
        }


    }

}
