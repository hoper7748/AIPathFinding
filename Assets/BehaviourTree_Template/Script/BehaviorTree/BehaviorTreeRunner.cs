using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree tree;

        private void Start()
        {
            tree = tree.Clone();
            tree.Bind(GetComponent<AIAgent>());

        }

        private void Update()
        {
            tree.Update();
        }
    }
}