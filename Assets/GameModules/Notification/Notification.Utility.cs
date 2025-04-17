using System;
using System.Collections.Generic;


namespace GameModules
{
    public partial class Notification
    {
        private static readonly List<IUINoticeable> Listeners = new List<IUINoticeable>();
        private static readonly Dictionary<Type, Notification> Container = new Dictionary<Type, Notification>();

        static Notification Register(Type type, params Notification[] parents)
        {
            return Register(type, null, parents);
        }

        static Notification Register(Type type, Func<bool> checkNotify = null, params Notification[] parents)
        {
            if (Container.TryGetValue(type, out var notification))
            {
                return notification;
            }

            notification = new Notification(type, checkNotify, parents);
            Container.Add(notification.Typekey, notification);
            return notification;
        }

        public static Notification Get(Type type)
        {
            Container.TryGetValue(type, out var ret);
            return ret;
        }

        public static bool AddListener(IUINoticeable listener)
        {
            if (listener == null)
            {
                return false;
            }

            if (Listeners.Contains(listener))
            {
                return false;
            }

            Listeners.Add(listener);
            Notification no = Get(listener.GetTypeKey());
            if (no != null)
            {
                listener.OnNotification(no.Count);
            }

            return true;
        }

        public static bool RemoveListener(IUINoticeable listener)
        {
            if (listener == null)
            {
                return false;
            }

            return Listeners.Remove(listener);
        }

        public static void ResetAllNotification()
        {
            foreach (var temp in Container)
            {
                temp.Value.Count = 0;
            }
        }
    }
}