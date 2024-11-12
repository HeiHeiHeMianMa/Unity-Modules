using UnityEditor;
using UnityEngine;

public class TestRightclick : EditorWindow
{
    [MenuItem("Test/右键点击")]
    public static void Showwindow()
    {
        GetWindow<TestRightclick>();
    }

    private void OnGUI()
    {
        // 直接用Button自身的点击事件，判断一下鼠标按钮
        if (GUILayout.Button(text: "Button1"))
        {
            if (Event.current.button == 1)
            {
                Debug.LogError("Right click Button1");
            }
            //else就是左键
        }

        // Button控件触发点击事件后，Event.current.Use()会被调用，此时Event.current.type会变成Used, GUILayoutUtility.GetLastRect()也不起作用了
        // 所以改用其他非交互控件，然后自己判断点击事件和点击范围，但是按钮的点击动画效果就没了。
        GUILayout.Label("Button2", "Button");
        // 看你自己喜欢用MouseDown还是MouseUp
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                Debug.LogError("Right click Button2");
            }
        }

        // 可以换一种方式拿到Rect: 通过Layout获取按钮范围
        Rect rectLabel = EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Button3", "Button");
        EditorGUILayout.EndHorizontal();
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            if (rectLabel.Contains(Event.current.mousePosition))
                Debug.LogError(message: "Right click Button3");
        }

        // 既然能拿到Rect，可以尝试一下用Button控件:
        // 但Event.current.type会变成Used的话，就无法区分MouseDown、MouseUp、MouseMove、MouseDrag等等了
        // 所以可以在GUILayout.Button前面获取type。
        EventType type1 = Event.current.type;
        Rect rectButton = EditorGUILayout.BeginHorizontal();
        GUILayout.Button("Button4", "Button");
        EditorGUILayout.EndHorizontal();
        if (type1 == EventType.MouseDown && Event.current.button == 1)
        {
            if (rectButton.Contains(Event.current.mousePosition))
            {
                Debug.LogError(message: "Right click Button4");
            }
        }

        //改用一个可以获得焦点的空间，给控件取个名称，然后判断控件名称
        EventType type2 = Event.current.type;
        GUI.SetNextControlName("MyButton");
        GUILayout.TextField("Button5", "Button");
        if (type2 == EventType.MouseDown && Event.current.button == 1)
        {
            if (GUI.GetNameOfFocusedControl() == "MyButton")
            {
                Debug.LogError(message: "Right click Button5");
            }
        }
        //阻止控件得到焦点，否则点击别的按钮，GUI.GetName0fFocusedcontrol()也可能还是“MyButton”。
        if (GUI.GetNameOfFocusedControl() == "MyButton")
        {
            GUI.FocusControl(name: null);
        }
    }
}