namespace GameModules.Notification
{
    public partial class Notification
    {
        /// <summary> 红点提示类型(新加必须加到最后) </summary>
        public enum Type
        {
            /// <summary> 默认 </summary>
            None = 0,
            
            A,
            A_1,
            A_2,
            A_3,
            
            B,
            B_1,
            B_1_1,

            MAX,
        }
    }
}