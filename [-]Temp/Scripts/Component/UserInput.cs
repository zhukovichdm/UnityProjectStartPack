using Scripts.Component.Actions;

namespace Scripts.Component
{
    /// <summary>
    /// События пользовательского ввода.
    /// </summary>
    public static class UserInput
    {
        public static readonly MyAction InputEscapeAction = new MyAction();
//        public static readonly MyAction InputMouseLeft = new MyAction();
//        public static readonly MyAction InputMouseRight = new MyAction();

        public static bool InputEscape;
        public static bool InputMouseLeft;
        public static bool InputMouseRight;
        public static float InputScrollWheel;
    }
}