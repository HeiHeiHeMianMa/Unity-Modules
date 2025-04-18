using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameModules
{
    public partial class Notification
    {
        private bool _needRefresh = false;
        private Func<bool> _checkNotify;

        private static Stopwatch _watch = new Stopwatch();
        private static long _maxTimeSlice = 10;
        private static Type _lastRefresh = Type.None;
        public static bool IsBusy => _watch.ElapsedMilliseconds >= _maxTimeSlice;

        /// <summary>
        /// 设置需要刷新，下一帧统一刷新
        /// </summary>
        /// <param name="eType"></param>
        public static void SetNeedRefresh(Type eType)
        {
            if (!Container.TryGetValue(eType, out var cNotification))
            {
                return;
            }

            if (null == cNotification)
            {
                return;
            }

            cNotification._needRefresh = true;
        }

        /// <summary>
        /// 需要立即刷新的调用此方法
        /// </summary>
        /// <param name="eType"></param>
        /// <returns></returns>
        public static bool RefreshNotification(Type eType)
        {
            if (!Container.TryGetValue(eType, out var cNotification))
                return false;
            if (cNotification?._checkNotify == null)
                return false;
            if (cNotification._needRefresh == false)
                return false;

            cNotification._needRefresh = false;

            int nNotifyCount = cNotification._checkNotify() ? 1 : 0;
            cNotification.Count = nNotifyCount;

            return true;
        }

        public static void Update()
        {
            _watch.Reset();
            _watch.Start();
            
            var t = _lastRefresh + 1;
            if (t >= Type.MAX)
            {
                t = Type.None;
            }

            for (; t < Type.MAX; t++)
            {
                if (IsBusy)
                {
                    Debug.Log($"{Time.frameCount}帧  繁忙 当帧耗时 {_watch.ElapsedMilliseconds} 等待下一帧");
                    break;
                }
                
                if (RefreshNotification(t))
                {
                    Debug.Log($"{Time.frameCount}帧  执行{t} 当帧耗时 {_watch.ElapsedMilliseconds}");
                }
                
                _lastRefresh = t;
            }
        }
    }
}