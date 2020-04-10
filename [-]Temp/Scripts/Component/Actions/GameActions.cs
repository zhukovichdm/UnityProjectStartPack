using Scripts.Data;
using UnityEngine;

namespace Scripts.Component.Actions
{
    /// <summary>
    /// Основные события.
    /// </summary>
    public static class GameActions
    {
        public static readonly MyAction<(bool, ControlObjectInformation)> CursorOverObject = new MyAction<(bool, ControlObjectInformation)>();
        public static readonly MyAction<(bool, GameObject, DataObjectInformation)> UpdateObjectInformation = new MyAction<(bool, GameObject, DataObjectInformation)>();
        public static readonly MyAction<(GameObject target, CameraModes cameraMode, DataScrollParameter scrollParameter, DataLimitationParameter limitationParameter)> LookAt = new MyAction<(GameObject, CameraModes, DataScrollParameter, DataLimitationParameter)>();

        public static readonly MyAction<ControlObjectInformation> SelectedObjectForTesting = new MyAction<ControlObjectInformation>();
    }
}