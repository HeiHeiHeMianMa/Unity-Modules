using GameModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    void Start()
    {
        Module.Init();
        
        Module.UI.Open(GameModule.UI.UIType.UIMain);
    }
}
