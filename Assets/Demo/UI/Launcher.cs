using GameModules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    void Start()
    {
        UIModule.Instance.Init();
        UIModule.Instance.Open(UIType.UIMain);
    }
}
