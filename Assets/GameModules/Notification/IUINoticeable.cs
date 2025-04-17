namespace GameModules
{
    public interface IUINoticeable
    {
        Notification.Type GetTypeKey();
        void OnNotification(int curCount);
    }
}
