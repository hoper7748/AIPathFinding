using UnityEngine;

namespace BehaviourTree
{
    public class RootNode : Node
    {
        public Node child;
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            try
            {
                return child.Update();
            }
            catch
            {
                Debug.Log("RootNode havn't ChildNode");
            }
            return State.Failure;
        }

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}