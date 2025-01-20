using System;
using System.Collections.Generic;
using UnityEngine;


public static class ZEvent
{
    private static readonly Dictionary<ZEventID, Delegate> eventDic = new ();
    
    
    #region 注册

    public static void AddEvent(this ZEventID eventName, Action registerFunc)
    {
        if (eventDic.TryGetValue(eventName, out var del))
        {
            if (del is Action act)
            {
                eventDic[eventName] = act + registerFunc;
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
        else
        {
            eventDic[eventName] = registerFunc;
        }
    }
    
    public static void AddEvent<T>(this ZEventID eventName, Action<T> registerFunc)
    {
        if (eventDic.TryGetValue(eventName, out var del))
        {
            if (del is Action<T> act)
            {
                eventDic[eventName] = act + registerFunc;
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
        else
        {
            eventDic[eventName] = registerFunc;
        }
    }

    public static void AddEvent<T1, T2>(this ZEventID eventName, Action<T1, T2> registerFunc)
    {
        if (eventDic.TryGetValue(eventName, out var del))
        {
            if (del is Action<T1, T2> act)
            {
                eventDic[eventName] = act + registerFunc;
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
        else
        {
            eventDic[eventName] = registerFunc;
        }
    }

    #endregion

    #region 移除
    
    public static void RemoveEvent(this ZEventID eventName, Action registerFunc)
    {
        if (eventDic.TryGetValue(eventName, out var del) && del != null)
        {
            if (del is Action act)
            {
                eventDic[eventName] = act - registerFunc;
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
    }

    public static void RemoveEvent<T>(this ZEventID eventName, Action<T> registerFunc)
    {
        if (eventDic.TryGetValue(eventName, out var del) && del != null)
        {
            if (del is Action<T> act)
            {
                eventDic[eventName] = act - registerFunc;
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
    }

    public static void RemoveEvent<T1, T2>(this ZEventID eventName, Action<T1, T2> registerFunc)
    {
        if (eventDic.TryGetValue(eventName, out var del) && del != null)
        {
            if (del is Action<T1, T2> act)
            {
                eventDic[eventName] = act - registerFunc;
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
    }

    #endregion

    #region 发送
    
    public static void DispatchEvent(this ZEventID eventName)
    {
        if (eventDic.TryGetValue(eventName, out var del) && del != null)
        {
            if (del is Action act)
            {
                act?.Invoke();
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配");
            }
        }
    }

    public static void DispatchEvent<T>(this ZEventID eventName, T param1)
    {
        if (eventDic.TryGetValue(eventName, out var del) && del != null)
        {
            if (del is Action<T> act)
            {
                act?.Invoke(param1);
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配 param1:{param1}");
            }
        }
    }

    public static void DispatchEvent<T1, T2>(this ZEventID eventName, T1 param1, T2 param2)
    {
        if (eventDic.TryGetValue(eventName, out var del) && del != null)
        {
            if (del is Action<T1, T2> act)
            {
                act?.Invoke(param1, param2);
            }
            else
            {
                Debug.LogError($"{eventName} 事件参数不匹配 param1:{param1} param2:{param2}");
            }
        }
    }

    #endregion
}