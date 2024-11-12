using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.EditorTools;
using Object = UnityEngine.Object;

[CustomEditor(typeof(pets))]
//用于使自定义编辑器支持多对象编辑的属性
[CanEditMultipleObjects]
public class Cusom_Pets : Editor
{
    AnimBool m_ShowExtraFields;
    private SerializedProperty NameProp;
    private SerializedProperty AgeProp;
    private SerializedProperty IdProp;
    private SerializedProperty Color_Prop;
    private SerializedProperty Pet_PositionProp;
    private SerializedProperty Pet_SleepProp;
    private SerializedProperty pet_guid_Prop;
    private pets Editor_Pets;
    string m_String;
    Color m_Color = Color.white;
    int m_Number = 0;
    private bool IS_BeginFoldoutHeaderGroup = true;
    private void OnEnable()
    {
        NameProp = RentProp("Name");
        AgeProp = RentProp("age");
        IdProp = RentProp("Id");
        Color_Prop = RentProp("Color_App");
        Pet_PositionProp = RentProp("Pet_Position");
        Pet_SleepProp = RentProp("Pet_Sleep");
        pet_guid_Prop = RentProp("Pet_GUID");
        Editor_Pets = target as pets;
        m_ShowExtraFields = new AnimBool(true);
        m_ShowExtraFields.valueChanged.AddListener(Repaint);
        addScroll = new List<string>();
    }
    Vector2 scrollPos;
    private List<string> addScroll;
    private bool IsToggle;
    bool[] skill = new bool[3] { false, false, false };
    private string _dopdownButton;
    private Pet_FlagsEnum Pet_FlagsEnum_iteml;
    private bool Is_EditorGUILayoutFoldout;
    private Gradient gradient = new Gradient();
    private bool IS_InspectorTitlebar;
    private int selectedSize = 1;
    string[] selectedSize_names = new string[] { "Normal", "Double", "Quadruple" };
    int[] selectedSize_sizes = { 1, 2, 4 };
    //遮罩
    static int options_flags = 0;
    static string[] options_mask = new string[] { "CanJump", "CanShoot", "CanSwim" };
    float minVal = -10;
    float minLimit = -20;
    float maxVal = 10;
    float maxLimit = 20;
    private string password;
    private Rect init_rect;
    private RectInt _rectInt;
    private float defult_Slider;
    private string defult_TextArea;

    bool aaa;
    Dictionary<string, bool> TTT = new Dictionary<string, bool>();
    public override void OnInspectorGUI()
    {
        //更新,始终在开始
        serializedObject.Update();
        //存在平台切换
        BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
        if (selectedBuildTargetGroup == BuildTargetGroup.Android)
        {
            foreach (var item in selectedSize_names)
            {
                TTT[item] = EditorGUILayout.BeginFoldoutHeaderGroup(TTT.ContainsKey(item) && TTT[item], item); EditorGUILayout.EndFoldoutHeaderGroup();
                if (TTT[item])
                {
                    EditorGUILayout.LabelField(item);
                }
            }

            EditorGUILayout.LabelField("Android 文本");
            //创建一个开关
            m_ShowExtraFields.target = EditorGUILayout.ToggleLeft("查看文本颜色", m_ShowExtraFields.target);

            //开始一个可以隐藏/显示的组，并将对过渡进行动画处理。
            if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded))
            {
                EditorGUILayout.PrefixLabel("Color");
                m_Color = EditorGUILayout.ColorField(m_Color);
                EditorGUILayout.PrefixLabel("Text");
                m_String = EditorGUILayout.TextField(m_String);
                EditorGUILayout.PrefixLabel("Number");
                m_Number = EditorGUILayout.IntSlider(m_Number, 0, 10);
                EditorGUILayout.Space(20);


            }
            //结束过渡动画
            EditorGUILayout.EndFadeGroup();
            //使用左侧的折叠箭头制作标签。
            IS_BeginFoldoutHeaderGroup = EditorGUILayout.BeginFoldoutHeaderGroup(IS_BeginFoldoutHeaderGroup, "宠物transform");
            if (IS_BeginFoldoutHeaderGroup)
            {
                //如果选中物体
                Pet_PositionProp.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("宠物的位置"), Pet_PositionProp.vector3Value);
            }
            //结束折叠
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.HelpBox("这是一个ScrollView", MessageType.None, true);
            //开始自动布局的滚动视图
            EditorGUILayout.Space(3);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(150), GUILayout.Height(50));
            for (int i = 0; i < addScroll.Count; i++)
            {
                GUILayout.Label(addScroll[i]);
            }
            //结束布局
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("增加文字"))
            {
                addScroll?.Add("妹妹,借我一百块");
            }


        }
        else if (selectedBuildTargetGroup == BuildTargetGroup.Standalone)
        {
            EditorGUILayout.HelpBox(new GUIContent("求助"), true);
            EditorGUILayout.Space(10);
            //开始垂直
            Rect r = EditorGUILayout.BeginHorizontal("按钮");
            if (GUI.Button(r, GUIContent.none))
            {
                Debug.Log("呵呵,想多了");
            }
            GUILayout.Label("点击我");
            GUILayout.Label("我告诉你答案");
            EditorGUILayout.EndHorizontal();
            //通过切换开始一个垂直组，以一次启用或禁用其中的所有控件。
            EditorGUILayout.Space(10);
            IsToggle = EditorGUILayout.BeginToggleGroup("这个宠物会的技能", IsToggle);
            skill[0] = EditorGUILayout.Toggle("跳舞", skill[0]);
            skill[1] = EditorGUILayout.Toggle("游泳", skill[1]);
            skill[2] = EditorGUILayout.Toggle("干饭", skill[2]);
            //结束切换组开关
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space(10);
            EditorGUILayout.BoundsField("宠物活动范围( BoundsField )", new Bounds(Vector3.zero, new Vector3(100, 100)));
            EditorGUILayout.Space(10);
            EditorGUILayout.BoundsIntField("宠物活动最小范围( BoundsIntField )", new BoundsInt(Vector3Int.zero, new Vector3Int(10, 10, 0)));
            EditorGUILayout.Space(10);
            //动画曲线 CurveField
            Pet_SleepProp.animationCurveValue = EditorGUILayout.CurveField("宠物睡眠曲线", Pet_SleepProp.animationCurveValue, Color.cyan, new Rect(new Vector2(5, 5), new Vector2(30, 200)));
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("这些宠物爱干嘛( EnumFlagsField)");
            Editor_Pets.PetFlagsEnum = (Pet_FlagsEnum)EditorGUILayout.EnumFlagsField(Pet_FlagsEnum_iteml);
            Editor_Pets.PetFlagsEnum = (Pet_FlagsEnum)EditorGUILayout.EnumFlagsField(Pet_FlagsEnum_iteml);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            Editor_Pets.PetFlagsEnum = (Pet_FlagsEnum)EditorGUILayout.EnumPopup("第二个enum", Editor_Pets.PetFlagsEnum);
            EditorGUILayout.Space(10);
            EditorGUILayout.ObjectField("宠物对象", null, typeof(Object), true);
            EditorGUILayout.Space(10);
            password = EditorGUILayout.PasswordField("密码输入", password);
            EditorGUILayout.Space(10);
            EditorGUILayout.SelectableLabel("可以复制粘贴的可读标签(SelectableLabel)");
            EditorGUILayout.Space(10);
            defult_Slider = EditorGUILayout.Slider("制作一个滑块(Slider)", defult_Slider, 0, 60);
            EditorGUILayout.Space(10);
            EditorGUILayout.TagField("可选择标签(TagField)");
            EditorGUILayout.Space(10);
            defult_TextArea = EditorGUILayout.TextArea(defult_TextArea, GUILayout.Height(30));

        }
        else
        {

            EditorGUILayout.Space(10);
            //显示自定义gui控件
            EditorGUILayout.IntSlider(AgeProp, 0, 20, new GUIContent("年龄"));
            //进度栏
            ProgressBar(AgeProp.intValue / 20.0f, "年龄");
            EditorGUILayout.IntSlider(IdProp, 0, 100, new GUIContent("编号"));
            ProgressBar(IdProp.intValue / 100.0f, "编号");
            //操作属性对象
            EditorGUILayout.PropertyField(NameProp, new GUIContent("宠物名字"));
            //应用更改  始终在末尾
            //垂直列表EndFadeGroup
            Rect r = (Rect)EditorGUILayout.BeginVertical("Button");
            //button
            if (GUI.Button(r, GUIContent.none))
                Debug.Log("呵呵,想多了");
            //文本
            GUILayout.Label("点击我");
            GUILayout.Label("我告诉你答案");
            EditorGUILayout.EndVertical();
            // EditorGUILayout.PropertyField(Color_Prop,new GUIContent("宠物颜色"));
            Color_Prop.colorValue = EditorGUILayout.ColorField("宠物颜色", Color_Prop.colorValue);
            //双精度文本
            pet_guid_Prop.doubleValue = EditorGUILayout.DoubleField("宠物GUID ( DoubleField )", pet_guid_Prop.doubleValue);
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("下拉内容 (DropdownButton):");
            if (EditorGUILayout.DropdownButton(new GUIContent(_dopdownButton), FocusType.Keyboard))
            {
                var alls = new string[4] { "小猫咪", "小狗狗", "小龟龟", "小鸡鸡" };
                //GenericMenu 用于创建自定义上下文按钮和下拉按钮
                GenericMenu _menu = new GenericMenu();

                alls.ToList().ForEach(x =>
                {
                    _menu.AddItem(new GUIContent(x), _dopdownButton.Equals(x), OnValueSelected, x);
                });

                _menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("平台工具使用 (EditorToolbar):");
            //工具栏 跟旋转平移一样
            EditorGUILayout.EditorToolbar(new EditorTool[] { new tool_at() });
            EditorGUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            Is_EditorGUILayoutFoldout = EditorGUILayout.Foldout(Is_EditorGUILayoutFoldout, "Foldout方法的使用");
            if (Is_EditorGUILayoutFoldout)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("使用左侧的折叠箭头制作标签");

            }
            EditorGUILayout.Space(10);
            gradient = EditorGUILayout.GradientField("渐变 由用户编辑的渐变(Gradient)", gradient);
            EditorGUILayout.Space(20);
            selectedSize = EditorGUILayout.IntPopup("制作一个整数弹出选择字段: ", selectedSize, selectedSize_names, selectedSize_sizes);
            EditorGUILayout.Space(20);
            selectedSize = EditorGUILayout.Popup("制作一个通用的弹出选择字段。", selectedSize, selectedSize_names);
            // EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Min Val(MinMaxSlider):", minVal.ToString());
            EditorGUILayout.LabelField("Max Val(MinMaxSlider):", maxVal.ToString());
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);
            EditorGUILayout.Space(20);
            // EditorGUILayout.EndHorizontal();
            EditorGUILayout.LayerField("添加一个层级", 0);
            EditorGUILayout.Space(20);
            options_flags = EditorGUILayout.MaskField("添加一个遮罩", options_flags, options_mask); EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("显示前缀标签的简单窗口。");
            Editor_Pets.age = EditorGUILayout.IntField(Editor_Pets.age);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            init_rect = EditorGUILayout.RectField(new GUIContent("RectField"), init_rect);
            EditorGUILayout.Space(10);
            _rectInt = EditorGUILayout.RectIntField("_rectInt", _rectInt);
        }
        EditorGUILayout.Space(30);
        //制作一个类似于检查器窗口的标题栏。PropertyField
        EditorGUILayout.LabelField("制作一个类似于检查器窗口的标题栏。");
        if (Selection.activeGameObject)
        {
            var selectedTransform = Selection.activeGameObject.transform;
            IS_InspectorTitlebar = EditorGUILayout.InspectorTitlebar(IS_InspectorTitlebar, selectedTransform);
            if (IS_InspectorTitlebar)
            {
                selectedTransform.position =
                    EditorGUILayout.Vector3Field("Position", selectedTransform.position);
                EditorGUILayout.Space();
                selectedTransform.localScale =
                    EditorGUILayout.Vector3Field("Scale", selectedTransform.localScale);
            }
        }


        EditorGUILayout.EndBuildTargetSelectionGrouping();
        serializedObject.ApplyModifiedProperties();
    }

    private SerializedProperty RentProp(string arg)
    {
        var iteml = serializedObject.FindProperty(arg);
        return iteml;
    }

    private void ProgressBar(float value, string label)
    {
        //设置边框
        Rect rect = GUILayoutUtility.GetRect(18, 18);
        //进度条
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space(10);

    }

    private void OnValueSelected(object arg)
    {
        _dopdownButton = arg.ToString();
    }
    [EditorTool("通用平台工具")]
    public class tool_at : EditorTool
    {
        GUIContent m_IconContent;
        void OnEnable()
        {
            Texture text = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Shader/texture/cili.png");
            m_IconContent = new GUIContent()
            {
                image = text,
                text = "平台工具",
                tooltip = "平台工具"
            };
        }
        //修改初始icon
        public override GUIContent toolbarIcon
        {
            get { return m_IconContent; }
        }
        public override void OnToolGUI(EditorWindow window)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 position = Tools.handlePosition;
            using (new Handles.DrawingScope(Color.green))
            {
                position = Handles.Slider(position, Vector3.right);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Vector3 delta = position - Tools.handlePosition;

                Undo.RecordObjects(Selection.transforms, "Move Platform");

                foreach (var transform in Selection.transforms)
                    transform.position += delta;
            }
        }
    }
}

