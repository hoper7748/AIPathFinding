
namespace BT
{
    public class Inverter : BT.Node
    {
        protected Node node;

        public Inverter(Node _node)
        {
            node = _node;
        }



        public override NodeState Evaluate()
        {
            switch(node.Evaluate())
            {
                case NodeState.Running:
                    nodeState = NodeState.Running;
                    break;
                case NodeState.Success:
                    nodeState = NodeState.Failure;
                    break;
                case NodeState.Failure:
                    nodeState = NodeState.Success;
                    break;
            }

            return nodeState;
        }
    }

}