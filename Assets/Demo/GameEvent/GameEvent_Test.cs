using System;
using System.Collections;
using System.Collections.Generic;
using GameModules;
using UnityEngine;

public class GameEvent_Test : MonoBehaviour
{
    void AAA()
    {
        Debug.LogError("AAA");
    }
    void BBB()
    {
        Debug.LogError("BBB");
    }

    private void OnGUI()
    {
        var index = 0;
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"注册A" ))
        {
            Debug.LogError("注册A");
            GameEventID.None.AddListener(AAA);
        }
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"移除A" ))
        {
            Debug.LogError("移除A");
            GameEventID.None.RemoveListener(AAA);
        }
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"注册B" ))
        {
            Debug.LogError("注册B");
            GameEventID.None.AddListener(BBB);
        }
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"移除B" ))
        {
            Debug.LogError("移除B");
            GameEventID.None.RemoveListener(BBB);
        }
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"广播" ))
        {
            GameEventID.None.Dispatch();
        }
    }
}
