using System;
using System.Collections.Generic;
using Pixeye.Unity;
using Scripts.Component;
using Scripts.Component.Actions;
using Scripts.Data;
using Scripts.Group1;
using Scripts.System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Behaviours.Other
{
    [Serializable]
    public struct PivotParameter
    {
        public GameObject pivot;
        public DataCameraParameters cameraParameters;
        public bool cameraToMaxDistance;
    }
    
    [RequireComponent(typeof(ControlObjectInformation))]
    public class OnClick : MonoBehaviour, IPointerClickHandler
    {
        [Foldout("Параметры для перемещения камеры.", true)]
        [SerializeField] private DataCameraParameters dataCameraParameters;
        [SerializeField] private List<PivotParameter> pivotParameters = new List<PivotParameter>();
//        [SerializeField] private List<GameObject> cameraPivots = new List<GameObject>();
        [SerializeField] private CameraModes cameraMode = CameraModes.LooksAt;

        [Foldout("Параметры выбора объекта.", true)] 
        [SerializeField] private bool ignoreClickOnChildren;
        [SerializeField] private bool updateObjectInformation;
        [SerializeField] private DataClickParameters clickParameters;

        [Foldout("Дополнительные параметры.", true)]
        [SerializeField] private ControlObjectInformation useThisObject;

        private Transform _mainCamera;
        private DataObjectInformation _dataObjectInformation;
        
        private void Awake()
        {
            _mainCamera = Camera.main.transform;
            _dataObjectInformation = GetComponent<ControlObjectInformation>().dataObjectInformation;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Cursor.lockState != CursorLockMode.None) return;
            if (eventData.pointerEnter != gameObject && ignoreClickOnChildren) return;
            if (eventData.button == clickParameters.inputButton && eventData.clickCount == clickParameters.clickCount)
                MoveCameraToPivot(0,true,true);
        }

        private void OnMouseOver()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            if (DoubleClick.Check(this, clickParameters.clickCount, (int) clickParameters.inputButton))
            {
                // Если ignoreClickOnChilds то нужно вызывать EventCall в объекте полученном через рейкаст.
                MoveCameraToPivot(0, true, true);
            }
        }

        private void MoveCameraToPivot(int id, bool cameraToMaxDistance, bool needReset)
        {
            if (Vector3.Distance(transform.position, _mainCamera.position) < clickParameters.maxDistanceToMainCamera ==
                false) return;
            GameActions.TempAction.Publish((this, pivotParameters.Count, needReset));

            GameActions.UpdateObjectInformation.Publish(useThisObject
                ? (updateObjectInformation, useThisObject.gameObject, useThisObject.dataObjectInformation)
                : (updateObjectInformation, gameObject, _dataObjectInformation));

            var pivot = pivotParameters.Count > 0 ? pivotParameters[id].pivot : gameObject;
            var data = pivotParameters[id].cameraParameters
                ? pivotParameters[id].cameraParameters
                : dataCameraParameters;

            GameActions.LookAt.Publish((pivot, cameraMode, data.dataScrollParameter, data.dataLimitationParameter,
                cameraToMaxDistance));
            
        }

        // При выборе пивота.
        public void MoveCameraToPivot(int id)
        {
            MoveCameraToPivot(id, pivotParameters[id].cameraToMaxDistance,false);
//            MoveCameraToPivot(id, false, false);
        }

        // При выборе объекта.
        public void MoveCameraToPivot()
        {
            MoveCameraToPivot(0, true, true);
        }
    }
}