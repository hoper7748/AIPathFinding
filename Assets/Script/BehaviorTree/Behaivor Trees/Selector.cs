using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace BT
{
    public class Selector : BT.Node
    {
        protected List<Node> nodes = new List<Node>();

        public Selector(List<Node> _nodes)
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
                        nodeState = NodeState.Running;
                        return nodeState;   
                    case NodeState.Success:
                        nodeState = NodeState.Success;
                        return nodeState;
                    case NodeState.Failure:
                        break;
                }
            }
            nodeState = NodeState.Failure;

            return nodeState;
        }
    }

}