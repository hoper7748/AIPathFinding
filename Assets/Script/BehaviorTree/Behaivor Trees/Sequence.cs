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

        public Sequence(List<Node> _nodes)
        {
            nodes = _nodes;
        }



        public override NodeState Evaluate()
        {
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