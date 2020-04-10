using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MyEditorExtensions : Editor
{
    [MenuItem("GameObject/MyCategory/GroupObject %g", false, 10)]
    public static void GroupObject()
    {
        var gameObjects = new List<Transform>(Selection.objects.Select(x => ((GameObject) x).transform));
        if (gameObjects.Count <= 1) return;

        var go = new GameObject().transform;
        Undo.RegisterCreatedObjectUndo(go.gameObject, "Created go");

        if (gameObjects[0].parent)
            go.parent = gameObjects[0].parent;

        Vector3 newPosition = new Vector3();
        foreach (var item in gameObjects)
            newPosition += item.position;
        newPosition /= gameObjects.Count;
        go.position = newPosition;

        foreach (var item in gameObjects)
            Undo.SetTransformParent(item, go, "Set new parent");

        Selection.activeObject = go;
        EditorUtility.SetDirty(go);
    }

    [MenuItem("GameObject/MyCategory/UnGroupObject %#g", false, 10)]
    public static void UnGroupObject()
    {
        var gameObjects = new List<Transform>(Selection.objects.Select(x => ((GameObject) x).transform));
        if (gameObjects.Count == 0) return;

        var newSelection = new List<GameObject>();
        foreach (var item in gameObjects)
        {
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(item)) continue;
            newSelection.Add(item.gameObject);
            while (item.childCount != 0)
            {
                newSelection.Add(item.GetChild(0).gameObject);
                Undo.SetTransformParent(item.GetChild(0), item.parent, "Set new parent");
                EditorUtility.SetDirty(item);
            }
        }
        Selection.objects = newSelection.ToArray();
    }
}