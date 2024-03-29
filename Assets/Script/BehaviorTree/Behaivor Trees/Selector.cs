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

        public Selector(List<Node> _nodes, string _name)
        {
            nodes = _nodes;
            name = _name;
        }



        public override NodeState Evaluate()
        {
            //Debug.Log($"{name}");
            foreach(var node in nodes)
            {
                switch(node.Evaluate())
                {
                    case NodeState.Running:
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