using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public delegate void TimerHandler(object[] args);

    public class Timer : SingletonMono<Timer>
    {
        internal class TimerInfo
        {
            public int timerId = 0;
            public float leftTime = 0;
            public float time = 0;
            public TimerHandler Handler;
            public bool isLoop = false;
            public bool isRunning = false;
            public bool isUnscaled = false; //是否使用非缩放的时间
            public object[] Args = null; //回调参数
        }

        private int curTimerId = 0;
        private readonly List<TimerInfo> timerList = new List<TimerInfo>();
        private readonly List<TimerInfo> unscaledTimerList = new List<TimerInfo>();

        /// <summary>
        /// 添加计时器。
        /// </summary>
        /// <param name="callback">计时器回调。</param>
        /// <param name="time">计时器间隔。</param>
        /// <param name="isLoop">是否循环。</param>
        /// <param name="isUnscaled">是否不收时间缩放影响。</param>
        /// <param name="args">传参。(避免闭包)</param>
        /// <returns>计时器Id。</returns>
        public int AddTimer(float time, TimerHandler callback, bool isLoop = false, bool isUnscaled = false, params object[] args)
        {
            TimerInfo timerInfo = new TimerInfo
            {
                timerId = ++curTimerId,
                leftTime = time,
                time = time,
                Handler = callback,
                isLoop = isLoop,
                isUnscaled = isUnscaled,
                Args = args,
                isRunning = true
            };

            InsertTimer(timerInfo);
            return timerInfo.timerId;
        }

        private void InsertTimer(TimerInfo timerInfo)
        {
            bool isInsert = false;
            if (timerInfo.isUnscaled)
            {
                for (int i = 0, len = unscaledTimerList.Count; i < len; i++)
                {
                    if (unscaledTimerList[i].leftTime > timerInfo.leftTime)
                    {
                        unscaledTimerList.Insert(i, timerInfo);
                        isInsert = true;
                        break;
                    }
                }

                if (!isInsert)
                {
                    unscaledTimerList.Add(timerInfo);
                }
            }
            else
            {
                for (int i = 0, len = timerList.Count; i < len; i++)
                {
                    if (timerList[i].leftTime > timerInfo.leftTime)
                    {
                        timerList.Insert(i, timerInfo);
                        isInsert = true;
                        break;
                    }
                }

                if (!isInsert)
                {
                    timerList.Add(timerInfo);
                }
            }
        }

        /// <summary>
        /// 暂停计时器。
        /// </summary>
        public void Pause(int timerId)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            if (timerInfo != null) timerInfo.isRunning = false;
        }

        /// <summary>
        /// 恢复计时器。
        /// </summary>
        public void Resume(int timerId)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            if (timerInfo != null) timerInfo.isRunning = true;
        }

        /// <summary>
        /// 计时器是否在运行中。
        /// </summary>
        public bool IsRunning(int timerId)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            return timerInfo is { isRunning: true };
        }

        /// <summary>
        /// 获得计时器剩余时间
        /// </summary>
        public float GetLeftTime(int timerId)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            if (timerInfo == null) return 0;
            return timerInfo.leftTime;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        public void Restart(int timerId)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            if (timerInfo != null)
            {
                timerInfo.leftTime = timerInfo.time;
                timerInfo.isRunning = true;
            }
        }

        public void ResetTimer(int timerId, TimerHandler callback, float time, bool isLoop = false, bool isUnscaled = false)
        {
            Reset(timerId, time, callback, isLoop, isUnscaled);
        }

        public void ResetTimer(int timerId, float time, bool isLoop, bool isUnscaled)
        {
            Reset(timerId, time, isLoop, isUnscaled);
        }

        /// <summary>
        /// 重置计时器。
        /// </summary>
        public void Reset(int timerId, float time, TimerHandler callback, bool isLoop = false, bool isUnscaled = false)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            if (timerInfo != null)
            {
                timerInfo.leftTime = time;
                timerInfo.time = time;
                timerInfo.Handler = callback;
                timerInfo.isLoop = isLoop;
                if (timerInfo.isUnscaled != isUnscaled)
                {
                    RemoveTimer(timerId);

                    timerInfo.isUnscaled = isUnscaled;
                    InsertTimer(timerInfo);
                }
            }
        }

        /// <summary>
        /// 重置计时器。
        /// </summary>
        public void Reset(int timerId, float time, bool isLoop, bool isUnscaled)
        {
            TimerInfo timerInfo = GetTimer(timerId);
            if (timerInfo != null)
            {
                timerInfo.leftTime = time;
                timerInfo.time = time;
                timerInfo.isLoop = isLoop;
                if (timerInfo.isUnscaled != isUnscaled)
                {
                    RemoveTimer(timerId);

                    timerInfo.isUnscaled = isUnscaled;
                    InsertTimer(timerInfo);
                }
            }
        }

        /// <summary>
        /// 移除计时器。
        /// </summary>
        /// <param name="timerId"></param>
        private void RemoveTimer(int timerId)
        {
            for (int i = 0, len = timerList.Count; i < len; i++)
            {
                if (timerList[i].timerId == timerId)
                {
                    timerList.RemoveAt(i);
                    return;
                }
            }

            for (int i = 0, len = unscaledTimerList.Count; i < len; i++)
            {
                if (unscaledTimerList[i].timerId == timerId)
                {
                    unscaledTimerList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 移除所有计时器。
        /// </summary>
        public void RemoveAllTimer()
        {
            timerList.Clear();
            unscaledTimerList.Clear();
        }

        private TimerInfo GetTimer(int timerId)
        {
            for (int i = 0, len = timerList.Count; i < len; i++)
            {
                if (timerList[i].timerId == timerId)
                {
                    return timerList[i];
                }
            }

            for (int i = 0, len = unscaledTimerList.Count; i < len; i++)
            {
                if (unscaledTimerList[i].timerId == timerId)
                {
                    return unscaledTimerList[i];
                }
            }

            return null;
        }

        private void LoopCallInBadFrame()
        {
            bool isLoopCall = false;
            for (int i = 0, len = timerList.Count; i < len; i++)
            {
                TimerInfo timerInfo = timerList[i];
                if (timerInfo.isLoop && timerInfo.leftTime <= 0)
                {
                    if (timerInfo.Handler != null)
                    {
                        timerInfo.Handler(timerInfo.Args);
                    }

                    timerInfo.leftTime += timerInfo.time;
                    if (timerInfo.leftTime <= 0)
                    {
                        isLoopCall = true;
                    }
                }
            }

            if (isLoopCall)
            {
                LoopCallInBadFrame();
            }
        }

        private void LoopCallUnscaledInBadFrame()
        {
            bool isLoopCall = false;
            for (int i = 0, len = unscaledTimerList.Count; i < len; i++)
            {
                TimerInfo timerInfo = unscaledTimerList[i];
                if (timerInfo.isLoop && timerInfo.leftTime <= 0)
                {
                    if (timerInfo.Handler != null)
                    {
                        timerInfo.Handler(timerInfo.Args);
                    }

                    timerInfo.leftTime += timerInfo.time;
                    if (timerInfo.leftTime <= 0)
                    {
                        isLoopCall = true;
                    }
                }
            }

            if (isLoopCall)
            {
                LoopCallUnscaledInBadFrame();
            }
        }

        private void UpdateTimer(float elapseSeconds)
        {
            bool isLoopCall = false;
            for (int i = 0, len = timerList.Count; i < len; i++)
            {
                TimerInfo timerInfo = timerList[i];

                if (!timerInfo.isRunning) continue;
                timerInfo.leftTime -= elapseSeconds;
                if (timerInfo.leftTime <= 0)
                {
                    if (timerInfo.Handler != null)
                    {
                        timerInfo.Handler(timerInfo.Args);
                    }

                    if (timerInfo.isLoop)
                    {
                        timerInfo.leftTime += timerInfo.time;
                        if (timerInfo.leftTime <= 0)
                        {
                            isLoopCall = true;
                        }
                    }
                    else
                    {
                        RemoveTimer(timerInfo.timerId);
                    }
                }
            }

            if (isLoopCall)
            {
                LoopCallInBadFrame();
            }
        }

        private void UpdateUnscaledTimer(float realElapseSeconds)
        {
            bool isLoopCall = false;
            for (int i = 0, len = unscaledTimerList.Count; i < len; i++)
            {
                TimerInfo timerInfo = unscaledTimerList[i];

                if (!timerInfo.isRunning) continue;
                timerInfo.leftTime -= realElapseSeconds;
                if (timerInfo.leftTime <= 0)
                {
                    if (timerInfo.Handler != null)
                    {
                        timerInfo.Handler(timerInfo.Args);
                    }

                    if (timerInfo.isLoop)
                    {
                        timerInfo.leftTime += timerInfo.time;
                        if (timerInfo.leftTime <= 0)
                        {
                            isLoopCall = true;
                        }
                    }
                    else
                    {
                        RemoveTimer(timerInfo.timerId);
                    }
                }
            }

            if (isLoopCall)
            {
                LoopCallUnscaledInBadFrame();
            }
        }

        public void Update()
        {
            UpdateTimer(Time.deltaTime);
            UpdateUnscaledTimer(Time.unscaledDeltaTime);
        }
    }
}