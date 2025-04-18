using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using GameModules;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIGenerateBind
{
    /// <summary>
    /// 类型注册 生成
    /// list中填的是别称/缩写，可以0或n个，全称默认添加不用手动加
    /// </summary>
    private static Dictionary<Type, List<string>> typeDic = new Dictionary<Type, List<string>>()
	{   
        {typeof(GameObject), new List<string>(){"go"}},
        {typeof(Transform), new List<string>(){"tr"}},
        {typeof(Animation), new List<string>(){"ani"}},
        {typeof(RectTransform), new List<string>(){ "rTr"}},
        {typeof(RawImage), new List<string>(){"rImg"}},
        {typeof(Image), new List<string>(){"img"}},
        //{typeof(Text), new List<string>(){"txt", "text"}},
        {typeof(TextMeshProUGUI), new List<string>(){"txt", "text"}},
        {typeof(Button), new List<string>(){"btn"}},
        {typeof(ScrollRect), new List<string>(){"sr", "sv", "scl"}},
        {typeof(Slider), new List<string>(){}},
        {typeof(List<GameObject>), new List<string>(){}},
	};
    
    private static List<BindInfo> bindInfoList = new List<BindInfo>();
    private static Dictionary<string, Type> nameDic = new Dictionary<string, Type>();
    private static string waitCompileKey = "CreateUIScript-WaitCompiling";
    private static string generateNamespace = "GameModules";
    private static string generateCSPath => $"{Application.dataPath}/Demo/UI/Generated/{{0}}.Bind.cs";
    
    
    [MenuItem("GameObject/UI工具/生成UI 绑定脚本", false, 1000)]
    public static void ExportScript()
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("警告", "选择一个ui", "确定");
            return;
        }
        
        Init();
        CreateScript(Selection.activeGameObject);
    }
    
    public static void Init()
    {
        foreach (KeyValuePair<Type, List<string>> kv in typeDic)
        {
            if (!kv.Value.Contains(kv.Key.Name))
            {
                kv.Value.Add(kv.Key.Name);
            }
        }
        nameDic.Clear();
        foreach (var kv in typeDic)
        {
            foreach (var name in kv.Value)
            {
                if (nameDic.ContainsKey(name))
                {
                    throw new Exception($"别名冲突：{name}");
                }
                nameDic.Add(name, kv.Key);
            }
        }
    }

    private static string GetClassName(GameObject targetObject)
    {
        return targetObject.name;
    }
    
    private static string GetCSFilePath(string className)
    {
        return string.Format(generateCSPath, className);
    }

    public static void CreateScript(GameObject control)
    {
        bindInfoList.Clear();
        ReadChildInfo(control, control.transform);
        
        var className = GetClassName(control);
        var path = GetCSFilePath(className);
        File.WriteAllText(path, generateScript(className), Encoding.UTF8);
        
        AssetDatabase.Refresh();
        
        Bind(control);
    }

    private static void Bind(GameObject control)
    {
        if (EditorApplication.isCompiling)
        {
            var insID = Selection.activeGameObject.GetInstanceID();
            var container = new InfoContainer{ InfoList = bindInfoList, SelInsID = insID};
            var json = JsonUtility.ToJson(container, true);
            PlayerPrefs.SetString(waitCompileKey, json);
            Debug.Log("等待编译");
        }
        else
        {
            AssignFields(control, GetClassName(control), bindInfoList);
        }
    }

    private static void ReadChildInfo(GameObject root, Transform tr)
    {
        foreach (Transform child in tr)
        {
            var split = child.name.Split('_');
            if (split.Length > 1)
            {
                var typeKey = split[^1];
                
                if (nameDic.ContainsKey(typeKey))
                {
                    bindInfoList.Add(new BindInfo(child.name, typeKey, buildGameObjectPath(root, child)));
                }
            }

            if (child.childCount > 0)
            {
                ReadChildInfo(root, child);
            }
        }
    }
    
    private static string buildGameObjectPath(GameObject root, Transform obj)
    {
        var buffer = new StringBuilder();

        while (obj != null && obj != root.transform)
        {
            if (buffer.Length > 0)
                buffer.Insert(0, "/");
            buffer.Insert(0, obj.name);
            obj = obj.parent;
        }
        return buffer.ToString();
    }
    
    private static string generateScript(string className)
    {
        var template = @"
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace @namespace
{
    public partial class @ClassName : UIViewBase
    {   
        @fields
    }
}
";
        template = template.Replace("@namespace", generateNamespace);
        template = template.Replace("@ClassName", className);
        var body1 = new StringBuilder();
        var fields = new StringBuilder();
        for (int i = 0; i < bindInfoList.Count; i++)
        {
            if (i > 0)
            {
                fields.Append("\r\n\t\t");
            }
            
            fields.Append(bindInfoList[i].FieldToCode());
        }
        template = template.Replace("@fields", fields.ToString()).Trim();
        return template;
    }

    [Serializable]
    private class InfoContainer
    {
        public List<BindInfo> InfoList;
        public int SelInsID;
    }
    [Serializable]
    public class BindInfo
    {
        public string name;
        public string type;
        public string path;
        public BindInfo(string name, string typeKey, string path)
        {
            this.name = name;
            this.path = path;
            this.type = nameDic[typeKey].Name;
        }
        
        public string FieldToCode()
        {
            return $"[SerializeField] private {type} {name};";
        }
    }
    
    [InitializeOnLoadMethod]
    private static void OnScriptReloaded()
    {
        EditorApplication.delayCall += TryBind;
    }
    private static void TryBind()
    {
        var json = PlayerPrefs.GetString(waitCompileKey);
        if (!string.IsNullOrEmpty(json))
        {
            Debug.Log("编译完成，开始绑定");
            var container = JsonUtility.FromJson<InfoContainer>(json);
            var selGo = Selection.activeGameObject;
            if (selGo != null && container.SelInsID == selGo.GetInstanceID())
            {
                AssignFields(selGo, GetClassName(selGo), container.InfoList);
                PlayerPrefs.DeleteKey(waitCompileKey);
            }
            else
            {
                Debug.LogError("编译后选中物体发生变化，重新选中后执行");
            }
        }
    }
    
    public static void AssignFields(GameObject targetObject, string componentName, List<BindInfo> list)
    {
        var targetComponent = targetObject.GetComponent(componentName);
        if (targetComponent == null)
        {
            //var type = Type.GetType($"{componentName}, Assembly-CSharp");
            var type = UIConfig.GetType($"{generateNamespace}.{componentName}");
            //var type = GetCSType(componentName);
            if (type == null || !typeof(Component).IsAssignableFrom(type))
            {
                Debug.LogError($"无法找到或添加组件 {componentName}！");
                return;
            }
            
            targetComponent = targetObject.AddComponent(type);
            Debug.Log($"已为目标对象 {targetObject.name} 添加组件 {componentName}");
        }
        
        var componentType = targetComponent.GetType();
        var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var info in list)
        {
            var field = fields.FirstOrDefault(f => (f.IsPublic || Attribute.IsDefined(f, typeof(SerializeField))) && f.Name == info.name);

            if (field == null)
            {
                Debug.LogError($"未在 {componentType.Name} 中找到字段：{info.name}");
                continue;
            }

            var childTransform = targetObject.transform.Find(info.path);
            if (childTransform == null)
            {
                Debug.LogError($"未找到路径为 {info.path} 的子对象！");
                continue;
            }

            if (info.type == "GameObject")
            {
                field.SetValue(targetComponent, childTransform.gameObject);
            }
            else
            {
                var childComponent = childTransform.GetComponent(info.type);
                if (childComponent != null)
                {
                    field.SetValue(targetComponent, childComponent);
                }
                else
                {
                    Debug.LogError($"{info.name} 上未找到类型为 {info.type} 的组件！");
                }
            }
        }
        
        EditorUtility.SetDirty(targetObject);
        Debug.Log("绑定完成");
    }

    private static Type GetCSType(string className)
    {
        var path = string.Format(generateCSPath, className).Replace(Application.dataPath, "Assets");
        var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>($"{path.Replace(Application.dataPath, "Assets")}");
        if (monoScript == null)
        {
            Debug.LogError($"无法加载脚本：{path}");
            return null;
        }
        var type = monoScript.GetClass();
        if (type == null)
        {
            Debug.LogError($"type == null");
            return null;
        }
        return type;
    }
}

