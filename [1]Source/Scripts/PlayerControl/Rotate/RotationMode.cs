using UnityEngine;

namespace Scripts.PlayerControl.Rotate
{
    public abstract class RotationMode : ScriptableObject
    {
        public KeyCode key = KeyCode.Mouse1;
        public bool invertY;
        public bool invertX;
        public float sens = 2;
        [Range(0, 1)] public float smooth = 0.1f;

        public bool useLimitationY;
        public Vector2 limitationY;
        public bool useLimitationX = true;
        public Vector2 limitationX = new Vector2(-90, 90);
        public Vector2 direction;
        protected Transform Body;
        protected Transform Cam;

        public virtual void Setup(Transform body, Transform cam)
        {
            Body = body;
            Cam = cam;

            direction = new Vector2(Cam.localRotation.eulerAngles.x, Body.rotation.eulerAngles.y);
        }

        public abstract void Invoke();
    }
}