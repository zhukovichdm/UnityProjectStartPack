using Scripts.Component;
using Scripts.Component.Actions;
using UnityEngine;

namespace Scripts.Behaviours
{
    public class ControlCursorVisibility : MonoBehaviour
    {
        [SerializeField] private GameObject aimImage;
        private bool _locked;

        private void Awake()
        {
            GameActions.LookAt.Subscribe(tuple => ChangeCursorLockMode_Subscriber(tuple.cameraMode));
        }

        private void Start()
        {
            UnLock();
        }

        private void ChangeCursorLockMode_Subscriber(CameraModes cameraMode)
        {
            switch (cameraMode)
            {
                case CameraModes.LooksAt:
                    UnLock();
                    break;
                case CameraModes.Player:
                    Lock();
                    break;
                case CameraModes.Free:
                    Lock();
                    break;
                case CameraModes.Pivot:
                    UnLock();
                    break;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                switch (Cursor.lockState)
                {
                    case CursorLockMode.None:
                        Lock();
                        break;
                    case CursorLockMode.Locked:
                        UnLock();
                        break;
                }
            }
        }

        public void UnlockCursor()
        {
            UnLock();
        }

        private void Lock()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (aimImage)
                aimImage.SetActive(true);
        }

        private void UnLock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (aimImage)
                aimImage.SetActive(false);
        }
    }
}