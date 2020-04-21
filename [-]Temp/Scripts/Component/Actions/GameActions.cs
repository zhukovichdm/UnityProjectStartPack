using Scripts.Behaviours.Other;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Component.Actions
{
    /// <summary>
    /// Основные события.
    /// </summary>
    public static class GameActions
    {
        public static readonly MyAction<(bool isEnter, ControlObjectInformation)> CursorOverObject = new MyAction<(bool, ControlObjectInformation)>();
        public static readonly MyAction<(bool updateObjectInformation, GameObject, DataObjectInformation)> UpdateObjectInformation = new MyAction<(bool, GameObject, DataObjectInformation)>();
        public static readonly MyAction<(GameObject target, CameraModes cameraMode, DataScrollParameter scrollParameter, DataLimitationParameter limitationParameter, bool cameraToMaxDistance)> LookAt = new MyAction<(GameObject, CameraModes, DataScrollParameter, DataLimitationParameter, bool)>();

        public static readonly MyAction<ControlObjectInformation> SelectedObjectForTesting = new MyAction<ControlObjectInformation>();
        
        public static readonly MyAction<(OnClick onClick, int countCameraPivots, bool needReset)> TempAction = new MyAction<(OnClick, int, bool)>();
    }
}