using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace BehaviourTree
{
    public class WeightDecoratorNode : DecoratorNode, IWeightNode
    {
        public UnityAction<float, float> onWeightChanged;

        // 이 노드로 들어가기 위한 가중치
        [SerializeField] private float weight;
        public float Weight
        {
            get { return weight; }
            set
            {
                if (weight != value)
                {
                    float original = weight;
                    weight = value;
                    onWeightChanged?.Invoke(original, weight);
                }
            }
        }

        public override Node Clone()
        {
            WeightDecoratorNode node = Instantiate(this);
            node.Weight = Weight;
            node.child = child.Clone();
            return node;
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {

            return child.Update();
        }
    }
}