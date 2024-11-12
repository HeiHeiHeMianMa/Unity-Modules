using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WindowTest : EditorWindow, IHasCustomMenu
{
    private void OnGUI()
    {
        this.Repaint();

        var e = Event.current;
        if (null != e)
        {
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                var genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("功能1"), false, () => { Debug.Log("功能1"); });
                genericMenu.AddItem(new GUIContent("功能合集/功能2"), false, () => { Debug.Log("功能2"); });
                genericMenu.AddItem(new GUIContent("功能合集/功能3"), false, () => { Debug.Log("功能3"); });
                genericMenu.AddSeparator("功能合集/");
                genericMenu.AddItem(new GUIContent("功能合集/功能4"), false, () => { Debug.Log("功能4"); });
                genericMenu.ShowAsContext();
            }
        }

        if (GUILayout.Button("测试自定义菜单"))
        {
            PopupWindow.Show(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), new PopupContent()
            {
                width = 130,
                Height = 100,
                OnGUIAction = popupRect =>
                {
                    popupRect.x += 6;
                    EditorGUI.LabelField(popupRect, "这里是通用弹窗示例。");
                    if (GUILayout.Button("测试自定义菜单"))
                    {
                        
                    }
                }
            });
        }
    }

    GUIStyle helpBtn;
    private void ShowButton(Rect rect)
    {
        var iconContent = EditorGUIUtility.IconContent("d_TerrainInspector.TerrainToolSplat On");

        helpBtn ??= new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset()
        };

        if (GUI.Button(rect, iconContent, helpBtn))
        {
            Debug.LogError("点击了ShowButton");
        }
    }
    
    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Edit"), false, () => 
        {
            Debug.LogError("点击了AddItemsToMenu");
        });
    }
    

    [MenuItem("Test/EditorWindow测试合集")]
    static void CreateWindow()
    {
        GetWindow(typeof(WindowTest), false, "WindowTest");
    }
}
