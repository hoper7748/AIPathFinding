using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace BT
{
    public class Sequence : BT.Node
    {
        protected List<Node> nodes = new List<Node>();

        public Sequence(List<Node> _nodes, string _name)
        {
            nodes = _nodes;
            name = _name;
        }

        public override NodeState Evaluate()
        {
            Debug.Log($"{name}");
            bool isAnyNodeRunning = false;
            foreach(var node in nodes)
            {
                switch(node.Evaluate())
                {
                    case NodeState.Running:
                        isAnyNodeRunning = true;
                        break;
                    case NodeState.Success:
                        break;
                    case NodeState.Failure:
                        nodeState = NodeState.Failure;
                        return nodeState;   
                }
            }
            nodeState = isAnyNodeRunning ? NodeState.Running : NodeState.Success;

            return nodeState;
        }
    }

}