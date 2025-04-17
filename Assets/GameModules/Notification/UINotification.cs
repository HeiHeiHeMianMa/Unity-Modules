using UnityEngine;
using UnityEngine.Serialization;

namespace GameModules
{
    public class UINotification : MonoBehaviour, IUINoticeable
    {
        [SerializeField] private GameObject redPoint;
        [SerializeField] private Notification.Type notificationType;

        void Awake()
        {
            if (redPoint != null)
            {
                redPoint.SetActiveEx(false);
            }
        }

        void OnEnable()
        {
            Notification.AddListener(this);
        }

        void OnDisable()
        {
            Notification.RemoveListener(this);
        }

        public void SetRedPoint(GameObject setRedObj)
        {
            redPoint = setRedObj;
            SetNotificationCount();
        }

        public void SetNotificationType(Notification.Type type)
        {
            if (notificationType != type)
            {
                notificationType = type;
                SetNotificationCount();
            }
        }

        public Notification.Type GetTypeKey()
        {
            return notificationType;
        }

        public void OnNotification(int curCount)
        {
            SetNotificationCount();
        }

        private void SetNotificationCount()
        {
            var no = Notification.Get(notificationType);
            if (null != no)
            {
                int count = no.Count;
                if (redPoint != null)
                {
                    if (no.IsShowRed)
                    {
                        redPoint.SetActiveEx(count > 0);
                    }
                    else
                    {
                        redPoint.SetActiveEx(false);
                    }
                }
            }
        }
    }
}