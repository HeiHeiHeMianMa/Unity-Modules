namespace GameModules.Notification
{
    public interface IUINoticeable
    {
        Notification.Type GetTypeKey();
        void OnNotification(int curCount);
    }
}
