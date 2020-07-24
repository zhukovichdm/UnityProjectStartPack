using UnityEngine;

public static class EditorGroup
{
    public delegate void ToBox();

    public static void SetColor(Color color, ToBox method)
    {
        var buf = GUI.backgroundColor;
        GUI.backgroundColor = color;
        method.Invoke();
        GUI.backgroundColor = buf;
    }
    
    public static void Vertical(ToBox method)
    {
        GUILayout.BeginVertical("");
        method.Invoke();
        GUILayout.EndVertical();
    }

    public static void VerticalBox(ToBox method)
    {
        GUILayout.BeginVertical("box");
        method.Invoke();
        GUILayout.EndVertical();
    }

    public static void Horizontal(ToBox method)
    {
        GUILayout.BeginHorizontal("");
        method.Invoke();
        GUILayout.EndHorizontal();
    }

    public static void HorizontalBox(ToBox method)
    {
        GUILayout.BeginHorizontal("box");
        method.Invoke();
        GUILayout.EndHorizontal();
    }
}