using UnityEngine;

namespace Scripts.Utilities.Editor
{
    public static class Box
    {
        public delegate void ToBox();

        public static void PutInVerticalBox(bool useGrouping, bool useBoxStyle, ToBox method)
        {
            if (useGrouping)
            {
                GUILayout.BeginVertical(useBoxStyle ? "box" : "");
                method.Invoke();
                GUILayout.EndVertical();
            }
            else
                method.Invoke();
        }

        public static void PutInHorizontalBox(bool useGrouping, bool useBoxStyle, ToBox method)
        {
            if (useGrouping)
            {
                GUILayout.BeginHorizontal(useBoxStyle ? "box" : "");
                method.Invoke();
                GUILayout.EndHorizontal();
            }
            else
                method.Invoke();
        }
    }
}