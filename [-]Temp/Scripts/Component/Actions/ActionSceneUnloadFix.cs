using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Component.Actions;

public class ActionSceneUnloadFix : MonoBehaviour
{
    private void OnDestroy()
    {
        GameActions.CursorOverObject.ClearListener();
        GameActions.UpdateObjectInformation.ClearListener();
        GameActions.LookAt.ClearListener();
        GameActions.SelectedObjectForTesting.ClearListener();
    }
}
