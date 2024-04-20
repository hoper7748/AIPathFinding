using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using BehaviourTree;


public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;
    
    public VisualTreeAsset BaseUXML;
    public StyleSheet BaseUSS;

    [MenuItem("BehaviorTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    // 에셋을 클릭하여 GUI Editor을 켜는 기능.
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        //var visualTree = BaseUXML;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree_Template/Editor/UIBuilder/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        //var styleSheet = BaseUSS;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree_Template/Editor/UIBuilder/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);


        treeView = root.Q<BehaviorTreeView>();
        treeView.SetEditor(this);
        inspectorView = root.Q<InspectorView>();
        blackboardView = root.Q<IMGUIContainer>();

        blackboardView.onGUIHandler = () =>
        {
            if (blackboardProperty == null) { return; }
            if (treeObject.targetObject == null) { return; }

            treeObject?.Update();
            EditorGUILayout.PropertyField(blackboardProperty);
            treeObject?.ApplyModifiedProperties();
        };

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    private void OnSelectionChange()
    {
        #region legarcy
        // Editor에서 선택한 BehaviorTree에 추가한 노드를 삽입함.
        BehaviorTree tree = Selection.activeObject as BehaviorTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                BehaviorTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                if (runner)
                {
                    tree = runner.tree;
                }
            }
        }

        if (Application.isPlaying)
        {
            // Editor가 실행 중일 때 빈 tree가 아닐 경우 ui editor를 보여줌.
            if (tree)
            {
                // 삽입한 노드를 뷰어에서 보여줌.
                treeView.PopulateView(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                // 삽입한 노드를 뷰어에서 보여줌.
                treeView.PopulateView(tree);
            }
        }
        //try
        //{
        //}
        //catch
        //{
        //    Debug.LogWarning("treeView is nullptr");
        //}
        if (tree != null)
        {
            treeObject = new SerializedObject(tree);
            blackboardProperty = treeObject.FindProperty("blackboard");
        }
        #endregion
        //BehaviorTree tree = Selection.activeObject as BehaviorTree;
        //if (!tree)
        //{
        //    if (Selection.activeGameObject)
        //    {
        //        var runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
        //        if (runner)
        //        {
        //            tree = runner.tree;
        //        }
        //    }
        //}

        //if (!tree) { return; }

        //if (Application.isPlaying ? true : AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        //{
        //    treeView?.PopulateView(tree);
        //}

        //treeObject = new SerializedObject(tree);
        //blackboardProperty = treeObject.FindProperty("blackboard");
    }

    void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeState();
    }
}