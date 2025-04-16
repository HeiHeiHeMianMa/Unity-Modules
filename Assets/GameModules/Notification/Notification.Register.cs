namespace GameModules.Notification
{
    public partial class Notification
    {
        public static void InitNotification()
        {
            // None.
            Register(Type.None);

            Notification A = Register(Type.A);
            Register(Type.A_1, Notification_Test.DoHeavyWork_8, A);
            Register(Type.A_2, Notification_Test.DoHeavyWork_8, A);
            Register(Type.A_3, Notification_Test.DoHeavyWork_8, A);
        }
    }
}
