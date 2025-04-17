using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{

    public enum UIType
    {
        UIMain,
        UIMessageWindow,
        UITestB,
        Max,
    }
    public class UIConfig
    {
        public static List<UIConfig> ConfigList = new List<UIConfig>()
        {
            new (UIType.UIMain, UILayer.NormalLayer, "Assets/Res/UI/UIMain/UIMain.prefab", false),
            new (UIType.UIMessageWindow, UILayer.NormalLayer, "Assets/Res/UI/UITest/UIMessageWindow.prefab", true),
            new (UIType.UITestB, UILayer.NormalLayer, "Assets/Res/UI/UITest/UITestB.prefab", false),
        };
        
        public string Path;
        public UIType UIType;
        public UILayer UILayer;
        public bool IsWindow;

        public UIConfig(UIType uiType, UILayer uiLayer, string path, bool isWindow = false)
        {
            this.UIType = uiType;
            this.UILayer = uiLayer;
            this.Path = path;
            this.IsWindow = isWindow;
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                type = Type.GetType(string.Format("{0}, {1}", typeName, assembly.FullName));
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}