using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

namespace BT
{
    [System.Serializable]
    public abstract class Node
    {
        public string name;

        // 추상 클래스
        protected NodeState nodeState;
        public NodeState getNodeState { get { 
                //Debug.Log($"{}")
                return nodeState; 
            } }

        public abstract NodeState Evaluate();


    }


    public enum NodeState
    {
        Running, Success, Failure
    }
}
