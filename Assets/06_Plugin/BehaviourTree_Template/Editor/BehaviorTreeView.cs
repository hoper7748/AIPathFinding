using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using BehaviourTree;
using Node = BehaviourTree.Node;

public class BehaviorTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }
    BehaviorTree tree;
    BehaviorTreeEditor treeEditor;

    public BehaviorTreeEditor getTreeEditor
    {
        get
        {
            return treeEditor;
        }
    }

    public BehaviorTreeView()
    {
        // Factory�� ���ϴ� Layer�� GridBackground�� ���� 
        Insert(0, new GridBackground());

        // ��� �߰� �κ�
        // �� �� �ƿ� ���.
        this.AddManipulator(new ContentZoomer());
        // ȭ�� �巡�� ���
        this.AddManipulator(new ContentDragger());
        // ��带 ���� �� �巡�׸� �� �� �ִ� ���
        this.AddManipulator(new SelectionDragger());
        // �巡�׷� �ڽ��� ����� �� �ȿ� ���� ������ ������ �� �ִ� ���.
        // ���� ����
        this.AddManipulator(new RectangleSelector());
        var BehaviourTreeEditor_uss = AssetDatabase.GUIDToAssetPath("0abae94caedf05944a403a050ef2358b");
        // StyleSheet �� ����
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(BehaviourTreeEditor_uss);
        styleSheets.Add(styleSheet);

        // ctrl + z, ctrl + y�� �����Ͽ��� ��, ������ �Լ��� ȣ����.
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    public void SetEditor(BehaviorTreeEditor editor)
    {
        treeEditor = editor;
    }

    public void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviorTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // Creates Node View.
        tree.nodes.ForEach(n => CreateNodeView(n, null));

        // Create Edges.
        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });

    }


    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // Tree ���ο� ��带 ������ ��, BehaviorTree object ���ο��� ��尡 ���� �Ǵ� ���.
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                // ����Ǿ��ִ� �������� ����(�ڽ�)�� �� �ִ� ��带 ����(�θ�)List���� �����ִ� ���.
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        // �����Ų �θ���κ��� ���� ��带 list�� �ҷ����̴� ���.
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.node, childView.node);
            });
        }

        // ����� �����ǿ� ���� ������� ����ǰ��ϴ� ���.
        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                NodeView view = n as NodeView;
                view.SortChildren();
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        var MousePosition = GetLocalMousePosition(evt.mousePosition);
        //base.BuildContextualMenu(evt);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, MousePosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, MousePosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, MousePosition));
            }
        }

    }

    void CreateNode(System.Type type, UnityEngine.Vector2? mousePosition)
    {
        Node node = tree.CreateNode(type);


        CreateNodeView(node, mousePosition);

    }

    void CreateNodeView(Node node, UnityEngine.Vector2? position)
    {
        NodeView nodeView = new NodeView(node, position);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeState()
    {
        nodes.ForEach(n =>
        {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }

    public UnityEngine.Vector2 GetLocalMousePosition(UnityEngine.Vector2 mousePosition, bool isSearchWindow = false)
    {
        if(treeEditor == null) { return UnityEngine.Vector2.zero; }

        UnityEngine.Vector2 worldMousePosition = mousePosition;

        if (isSearchWindow)
        {
            worldMousePosition -= treeEditor.position.position;
        }

        UnityEngine.Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

        return localMousePosition;
    }

}