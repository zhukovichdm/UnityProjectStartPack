using System;
using System.Collections.Generic;
using Scripts.System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Behaviours
{
    public class ControlAnimator : MonoBehaviour, IPointerClickHandler
    {
        public bool useInputButton;
        public PointerEventData.InputButton inputButton;
        public List<SystemAnimator> systemAnimators = new List<SystemAnimator>();

        private void Awake()
        {
            foreach (var systemAnimator in systemAnimators)
            {
                systemAnimator.Initialize(this);
                systemAnimator.UpdateParameters();
            }
        }

        private void OnEnable()
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.OnEnable();
        }

        // Использовалось раньше.
        public void SetSignAll(bool value)
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.SetSignAndPlayback(value ? AnimationDirection.ToEnd : AnimationDirection.ToBeginning);
        }

        public void ReverseAll()
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.ReverseAndPlayback();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Cursor.lockState != CursorLockMode.None) return;
            if (eventData.button == inputButton && useInputButton)
                ReverseAll();
        }

        private void OnMouseOver()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;
            if (Input.GetMouseButtonDown((int) inputButton))
                ReverseAll();
        }
    }
}