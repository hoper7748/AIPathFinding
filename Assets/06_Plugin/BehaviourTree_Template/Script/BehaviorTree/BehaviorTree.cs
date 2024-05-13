using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourTree
{
    [CreateAssetMenu()]
    public class BehaviorTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new List<Node>();
        public Blackboard blackboard = new Blackboard();
        public Node.State Update()
        {
            if (rootNode.state == Node.State.Running)
            {
                treeState = rootNode.Update();
            }
            return treeState;
        }

#if UNITY_EDITOR
        public Node CreateNode(System.Type type)
        {
            Vector2 mousePos = Input.mousePosition;
            // 새 노드 생성
            Node node = ScriptableObject.CreateInstance(type) as Node;
            // 이름 설정
            node.name = type.Name;
            //node.position = mousePos;
            node.guid = GUID.Generate().ToString();


            Undo.RecordObject(this, "BehaviourTree Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            //AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCompleteObjectUndo(node, "BehaviourTree Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;

        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "BehaviourTree Tree (CreateNode)");
            // 노드 삭제
            nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();


        }

        public void AddChild(Node parent, Node child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator != null)
            {
                Undo.RecordObject(decorator, "BehaviourTree Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "BehaviourTree Tree (AddChild)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null)
            {
                Undo.RecordObject(composite, "BehaviourTree Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }


        }

        public void RemoveChild(Node parent, Node child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator != null)
            {
                Undo.RecordObject(decorator, "BehaviourTree Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "BehaviourTree Tree (RemoveChild)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null)
            {
                Undo.RecordObject(composite, "BehaviourTree Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator && decorator.child != null)
            {
                children.Add(decorator.child);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode && rootNode.child != null)
            {
                children.Add(rootNode.child);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null)
            {
                return composite.children;
            }

            return children;
        }
#endif
        public void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n) =>
            {
                tree.nodes.Add(n);
            });
            return tree;

        }

        public void Bind(AIAgent agent)
        {
            Traverse(rootNode, node =>
            {
                node.agent = agent;
                node.blackboard = blackboard;
            });
        }
    }
}