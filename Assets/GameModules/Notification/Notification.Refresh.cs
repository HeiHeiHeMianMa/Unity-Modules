using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameModules
{
    public partial class Notification
    {
        private bool needRefresh = false;
        private Func<bool> checkNotify;

        private static Stopwatch watch = new Stopwatch();
        public static long maxTimeSlice = 10;
        private static Type _lastRefresh = Type.None;
        public static bool IsBusy => watch.ElapsedMilliseconds >= maxTimeSlice;

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

            cNotification.needRefresh = true;
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
            if (cNotification?.checkNotify == null)
                return false;
            if (cNotification.needRefresh == false)
                return false;

            cNotification.needRefresh = false;

            int nNotifyCount = cNotification.checkNotify() ? 1 : 0;
            cNotification.Count = nNotifyCount;

            return true;
        }

        public static void Update()
        {
            watch.Reset();
            watch.Start();
            
            var t = _lastRefresh + 1;
            if (t >= Type.MAX)
            {
                t = Type.None;
            }

            for (; t < Type.MAX; t++)
            {
                if (IsBusy)
                {
                    //Debug.Log($"{Time.frameCount}帧  繁忙 当帧耗时 {watch.ElapsedMilliseconds} 等待下一帧");
                    break;
                }
                
                if (RefreshNotification(t))
                {
                    //Debug.Log($"{Time.frameCount}帧  执行{t} 当帧耗时 {watch.ElapsedMilliseconds}");
                }
                
                _lastRefresh = t;
            }
        }
    }
}