using Scripts.Component.Actions;
using Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Behaviours.Other
{
    // NOTE: Как исправить OnPointer не работающий с CursorLockMode.Locked https://answers.unity.com/questions/1262549/cursorlockstate-and-onpointerenter-not-working-tog.html 

    [RequireComponent(typeof(ControlObjectInformation))]
    public class OnCursorOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool ignoreChildren;
        private ControlObjectInformation _controlObjectInformation;

        private void Awake()
        {
            _controlObjectInformation = GetComponent<ControlObjectInformation>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Cursor.lockState != CursorLockMode.None) return;
            if (eventData.pointerEnter == gameObject || !ignoreChildren)
                GameActions.CursorOverObject.Publish((true, _controlObjectInformation));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Cursor.lockState != CursorLockMode.None) return;
            if (eventData.pointerEnter == gameObject || !ignoreChildren)
                GameActions.CursorOverObject.Publish((false, _controlObjectInformation));
        }

        private void OnMouseEnter()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;
            GameActions.CursorOverObject.Publish((true, _controlObjectInformation));
        }

        private void OnMouseExit()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;
            GameActions.CursorOverObject.Publish((false, _controlObjectInformation));
        }
    }
}