using Scripts.Component.Actions;
using Scripts.Data;
using UnityEngine;

public class DebugOnClickAndOnCursorOver : MonoBehaviour
{
    private void Awake()
    {
        GameActions.UpdateObjectInformation.Subscribe(CursorOverObject);
        GameActions.CursorOverObject.Subscribe(CursorOverObject);
    }

    private void CursorOverObject((bool, GameObject, DataObjectInformation) tuple)
    {
        Debug.Log(tuple.Item3.name);
        Debug.Log(tuple.Item3.shortDescription);
    }

    private void CursorOverObject((bool isEnter, ControlObjectInformation) tuple)
    {
        Debug.Log(tuple.isEnter + "   " + tuple.Item2.dataObjectInformation.name);
    }
}