using GameModules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    void Start()
    {
        GameModule.Init();
        
        GameModule.UI.Open(UIType.UIMain);
    }
}
