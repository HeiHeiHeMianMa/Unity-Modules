using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules.Notification
{
    public partial class Notification
    {
        private int count;
        private List<Notification> parents = new List<Notification>();

        public int Count
        {
            get => count;
            private set => ChangeCount(value - count);
        }

        public Type Typekey { get; }

        public bool IsShowRed { get; private set; } = true;

        private Notification(Type typekey, Func<bool> checkNotify, params Notification[] parents)
        {
            Typekey = typekey;
            count = 0;
            this.checkNotify = checkNotify;

            foreach (var parent in parents)
            {
                if (parent.checkNotify != null)
                {
                    Debug.LogError($"{parent.Typekey} 为叶子节点，不能作为parent");
                    return;
                }
            }

            this.parents.AddRange(parents);
        }

        public void SetShowRed(bool isShowRed)
        {
            IsShowRed = isShowRed;
            foreach (var l in Listeners)
            {
                if (l.GetTypeKey().Equals(Typekey))
                {
                    l.OnNotification(this.Count);
                }
            }
        }

        private void ChangeCount(int diff)
        {
            if (diff == 0)
            {
                return;
            }

            count += diff;
            if (count <= 0)
            {
                count = 0;
            }

            if (null != parents)
            {
                foreach (var p in parents)
                {
                    p.ChangeCount(diff);
                }
            }

            foreach (var l in Listeners)
            {
                if (l.GetTypeKey().Equals(Typekey))
                {
                    l.OnNotification(this.Count);
                }
            }
        }
    }
}