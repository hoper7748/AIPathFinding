using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using BehaviourTree;
using Node = BehaviourTree.Node;
public class NodeView : UnityEditor.Experimental.GraphView.Node
{

    public Action<NodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;
    public NodeView(Node node, Vector2? position) : base(AssetDatabase.GUIDToAssetPath("55595d16967a82e4081e0e1691758658"))
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        // style(���)�� left �� top�� ��ġ�� node�� pos.x, y���� ��ġ�� ��.
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));

        if (position.HasValue) 
        {
            SetPosition(new Rect(position.Value, Vector2.zero));
        }

    }

    private void SetupClasses()
    {
        if (node is ActionNode)
            AddToClassList("action");
        else if (node is CompositeNode)
            AddToClassList("composite");
        else if (node is DecoratorNode)
            AddToClassList("decorator");
        else if (node is RootNode)
            AddToClassList("root");
    }
    private void CreateOutputPorts()
    {

        if (node is ActionNode) { }
        else if (node is CompositeNode)
            output = InstantiateCustomPort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        else if (node is DecoratorNode)
            output = InstantiateCustomPort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        else if (node is RootNode)
            output = InstantiateCustomPort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

        if (output != null)
        {
            output.portName = "";
            //output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    private void CreateInputPorts()
    {
        if (node is ActionNode)
            input = InstantiateCustomPort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        else if (node is CompositeNode)
            input = InstantiateCustomPort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        else if (node is DecoratorNode)
            input = InstantiateCustomPort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        else if (node is RootNode) { }

        if (input != null)
        {
            input.portName = "";
            //input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }


    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behavior Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public void SortChildren()
    {
        CompositeNode composite = node as CompositeNode;
        if (composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");
        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case Node.State.Running:
                    if (node.started)
                        AddToClassList("running");
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
        public virtual Port InstantiateCustomPort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            return CustomPort.Create<Edge>(orientation, direction, capacity, type);
        }
}