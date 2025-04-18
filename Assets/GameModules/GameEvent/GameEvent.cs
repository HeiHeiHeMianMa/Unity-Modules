using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public static class GameEvent
    {
        private static readonly Dictionary<GameEventID, List<Delegate>> eventDic = new();

        
        #region 注册
        public static void AddListener(this GameEventID eventID, Action callback) => AddInternal(eventID, callback);
        public static void AddListener<T>(this GameEventID eventID, Action<T> callback) => AddInternal(eventID, callback);
        public static void AddListener<T1, T2>(this GameEventID eventID, Action<T1, T2> callback) => AddInternal(eventID, callback);
        private static void AddInternal(GameEventID eventID, Delegate callback)
        {
            if (!eventDic.TryGetValue(eventID, out var list))
            {
                list = new List<Delegate>();
                eventDic[eventID] = list;
            }

            if (!list.Contains(callback))
                list.Add(callback);
        }
        #endregion

        #region 移除

        public static void RemoveListener(this GameEventID eventID, Action callback) => RemoveInternal(eventID, callback);
        public static void RemoveListener<T>(this GameEventID eventID, Action<T> callback) => RemoveInternal(eventID, callback);
        public static void RemoveListener<T1, T2>(this GameEventID eventID, Action<T1, T2> callback) => RemoveInternal(eventID, callback);
        private static void RemoveInternal(GameEventID eventID, Delegate callback)
        {
            if (eventDic.TryGetValue(eventID, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                    eventDic.Remove(eventID);
            }
        }
        #endregion

        #region 分发

        public static void Dispatch(this GameEventID eventID)
        {
            if (eventDic.TryGetValue(eventID, out var list))
            {
                foreach (var cb in list)
                {
                    if (cb is Action action)
                        action.Invoke();
                    else
                        Debug.LogError($"事件参数类型不匹配 {eventID}, 回调={cb.Method.DeclaringType}.{cb.Method.Name}");
                }
            }
        }

        public static void Dispatch<T>(this GameEventID eventID, T param1)
        {
            if (eventDic.TryGetValue(eventID, out var list))
            {
                foreach (var cb in list)
                {
                    if (cb is Action<T> action)
                        action.Invoke(param1);
                    else
                        Debug.LogError($"事件参数类型不匹配: {eventID}, 回调={cb.Method.DeclaringType}.{cb.Method.Name}");
                }
            }
        }

        public static void Dispatch<T1, T2>(this GameEventID eventID, T1 param1, T2 param2)
        {
            if (eventDic.TryGetValue(eventID, out var list))
            {
                foreach (var cb in list)
                {
                    if (cb is Action<T1, T2> action)
                        action.Invoke(param1, param2);
                    else
                        Debug.LogError($"事件参数类型不匹配 {eventID}, 回调={cb.Method.DeclaringType}.{cb.Method.Name}");
                }
            }
        }

        #endregion
    }
}
