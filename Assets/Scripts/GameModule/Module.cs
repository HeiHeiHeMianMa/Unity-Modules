using System.Collections;
using System.Collections.Generic;
using GameModule.UI;
using UnityEngine;

namespace GameModule
{
    public static class Module
    {
        public static UIModule UI { get; private set; }
        
        public static void Init()
        {
            UI = new UIModule();
            UI.Init();
        }
    }
}
