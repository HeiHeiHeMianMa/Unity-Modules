using System.Collections;
using System.Collections.Generic;
using GameModules;
using UnityEngine;

namespace GameModules
{
    public static class GameModule
    {
        public static UIModule UI { get; private set; }
        
        public static void Init()
        {
            UI = new UIModule();
            UI.Init();
        }
    }
}
