using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public partial class Notification
    {
        private int _count;
        private List<Notification> _parents = new List<Notification>();

        public int Count
        {
            get => _count;
            private set => ChangeCount(value - _count);
        }

        public Type Typekey { get; }

        public bool IsShowRed { get; private set; } = true;

        private Notification(Type typekey, Func<bool> checkNotify, params Notification[] parents)
        {
            Typekey = typekey;
            _count = 0;
            this._checkNotify = checkNotify;

            foreach (var parent in parents)
            {
                if (parent._checkNotify != null)
                {
                    Debug.LogError($"{parent.Typekey} 为叶子节点，不能作为parent");
                    return;
                }
            }

            this._parents.AddRange(parents);
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

            _count += diff;
            if (_count <= 0)
            {
                _count = 0;
            }

            if (null != _parents)
            {
                foreach (var p in _parents)
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