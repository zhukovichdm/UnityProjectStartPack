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
    [RequireComponent(typeof(ControlObjectInformation))]
    public class OnClick : MonoBehaviour, IPointerClickHandler
    {
        [Foldout("Параметры для перемещения камеры.", true)] 
        [SerializeField] private GameObject cameraPivot;
        [SerializeField] private CameraModes cameraMode = CameraModes.LooksAt;

        [Foldout("Основные параметры.", true)] 
        [SerializeField] private bool ignoreClickOnChildren;
        [SerializeField] private bool updateObjectInformation;
        [SerializeField] private DataClickParameters clickParameters;
        [SerializeField] private DataCameraParameters dataCameraParameters;

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
            if (eventData.pointerEnter == gameObject || !ignoreClickOnChildren)
                if (eventData.button == clickParameters.inputButton &&
                    eventData.clickCount == clickParameters.clickCount)
                    EventCall();
        }

        private void OnMouseOver()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            if (DoubleClick.Check(this, clickParameters.clickCount, (int) clickParameters.inputButton))
            {
                // Если ignoreClickOnChilds то нужно вызывать EventCall в объекте полученном через рейкаст.
                EventCall();
            }
        }

        private void EventCall()
        {
            if (Vector3.Distance(transform.position, _mainCamera.position) < clickParameters.maxDistanceToMainCamera ==
                false) return;

            GameActions.UpdateObjectInformation.Publish(useThisObject
                ? (updateObjectInformation, useThisObject.gameObject, objectInformation: useThisObject.dataObjectInformation)
                : (updateObjectInformation, gameObject, objectInformation: _dataObjectInformation));

            GameActions.LookAt.Publish((cameraPivot ? cameraPivot : gameObject, cameraMode,
                dataCameraParameters.dataScrollParameter, dataCameraParameters.dataLimitationParameter));
        }
    }
}