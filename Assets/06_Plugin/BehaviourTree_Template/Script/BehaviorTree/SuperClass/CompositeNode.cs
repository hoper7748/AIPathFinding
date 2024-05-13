using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        /*[HideInInspector]*/
        public List< Node> children = new List<Node>();

        public UnityAction<Node> onChildAdded;
        public UnityAction<Node> onChildRemoved;
        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c=>c.Clone());
            return node;
        }

    }
}
