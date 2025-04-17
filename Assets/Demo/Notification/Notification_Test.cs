using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameModules;
using UnityEngine;
using UnityEngine.UI;

public class Notification_Test : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 30;
        // 初始化红点系统
        Notification.InitNotification();
    }

    private void Update()
    {
        Notification.Update();
    }
    
    public void RefreshNotification()
    {
        Notification.SetNeedRefresh(Notification.Type.A_1);
        Notification.SetNeedRefresh(Notification.Type.A_2);
        Notification.SetNeedRefresh(Notification.Type.A_3);
    }

    public void ResetNotifications()
    {
        Notification.ResetAllNotification(); // 重置所有红点计数
    }

    private static Stopwatch stopwatch = new Stopwatch();
    public static bool DoHeavyWork_8() => DoHeavyWork(4);
    public static bool DoHeavyWork_20() => DoHeavyWork(20);
    public static bool DoHeavyWork(int milliseconds)
    {
        stopwatch.Reset();
        stopwatch.Start();
        bool shouldExit = false;
        while (!shouldExit)
        {
            double dummy = 0;
            for (int i = 0; i < 1000; i++)
            {
                dummy += Math.Sqrt(i) * Math.Sin(i);
                if (stopwatch.ElapsedMilliseconds >= milliseconds)
                {
                    shouldExit = true;
                    break;
                }
            }
        }
        return true;
    }
}
