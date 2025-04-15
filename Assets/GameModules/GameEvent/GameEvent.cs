using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameModules.GameEvent
{
    public static class GameEvent
    {
        private static readonly Dictionary<GameEventID, Delegate> eventDic = new();

        #region 注册

        public static void AddListener(this GameEventID eventID, Action registerFunc)
        {
            if (eventDic.TryGetValue(eventID, out var del) && del != null)
            {
                if (del is Action act)
                {
                    eventDic[eventID] = act + registerFunc;
                }
                else
                {
                    Debug.LogError($"{eventID} 事件参数不匹配");
                }
            }
            else
            {
                eventDic[eventID] = registerFunc;
            }
        }

        public static void AddListener<T>(this GameEventID eventID, Action<T> registerFunc)
        {
            if (eventDic.TryGetValue(eventID, out var del) && del != null)
            {
                if (del is Action<T> act)
                {
                    eventDic[eventID] = act + registerFunc;
                }
                else
                {
                    Debug.LogError($"{eventID} 事件参数不匹配");
                }
            }
            else
            {
                eventDic[eventID] = registerFunc;
            }
        }

        public static void AddListener<T1, T2>(this GameEventID eventID, Action<T1, T2> registerFunc)
        {
            if (eventDic.TryGetValue(eventID, out var del) && del != null)
            {
                if (del is Action<T1, T2> act)
                {
                    eventDic[eventID] = act + registerFunc;
                }
                else
                {
                    Debug.LogError($"{eventID} 事件参数不匹配");
                }
            }
            else
            {
                eventDic[eventID] = registerFunc;
            }
        }

        #endregion

        #region 移除

        public static void RemoveListener(this GameEventID eventID, Action registerFunc)
        {
            if (eventDic.TryGetValue(eventID, out var del) && del != null)
            {
                if (del is Action act)
                {
                    eventDic[eventID] = act - registerFunc;
                }
                else
                {
                    Debug.LogError($"{eventID} 事件参数不匹配");
                }
            }
        }

        public static void RemoveListener<T>(this GameEventID eventID, Action<T> registerFunc)
        {
            if (eventDic.TryGetValue(eventID, out var del) && del != null)
            {
                if (del is Action<T> act)
                {
                    eventDic[eventID] = act - registerFunc;
                }
                else
                {
                    Debug.LogError($"{eventID} 事件参数不匹配");
                }
            }
        }

        public static void RemoveListener<T1, T2>(this GameEventID eventID, Action<T1, T2> registerFunc)
        {
            if (eventDic.TryGetValue(eventID, out var del) && del != null)
            {
                if (del is Action<T1, T2> act)
                {
                    eventDic[eventID] = act - registerFunc;
                }
                else
                {
                    Debug.LogError($"{eventID} 事件参数不匹配");
                }
            }
        }

        #endregion

        #region 发送

        public static void Dispatch(this GameEventID evenID)
        {
            if (eventDic.TryGetValue(evenID, out var del) && del != null)
            {
                if (del is Action act)
                {
                    act?.Invoke();
                }
                else
                {
                    Debug.LogError($"{evenID} 事件参数不匹配");
                }
            }
        }

        public static void Dispatch<T>(this GameEventID evenID, T param1)
        {
            if (eventDic.TryGetValue(evenID, out var del) && del != null)
            {
                if (del is Action<T> act)
                {
                    act?.Invoke(param1);
                }
                else
                {
                    Debug.LogError($"{evenID} 事件参数不匹配 param1:{param1}");
                }
            }
        }

        public static void Dispatch<T1, T2>(this GameEventID evenID, T1 param1, T2 param2)
        {
            if (eventDic.TryGetValue(evenID, out var del) && del != null)
            {
                if (del is Action<T1, T2> act)
                {
                    act?.Invoke(param1, param2);
                }
                else
                {
                    Debug.LogError($"{evenID} 事件参数不匹配 param1:{param1} param2:{param2}");
                }
            }
        }

        #endregion
    }
}