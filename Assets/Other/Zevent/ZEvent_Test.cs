using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZEvent_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AAA()
    {
        Debug.LogError("AAA");
    }
    void BBB()
    {
        Debug.LogError("AAA");
    }

    private void OnGUI()
    {
        var index = 0;
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"注册" ))
        {
            ZEventID.None.AddEvent(AAA);
            Debug.LogError("注册");
        }
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"移除" ))
        {
            ZEventID.None.RemoveEvent(AAA);
            Debug.LogError("移除");
        }
        if (GUI.Button(new Rect(0, index++ * 100, 200, 100),"广播" ))
        {
            ZEventID.None.DispatchEvent();
            Debug.LogError("广播");
        }
    }
}
