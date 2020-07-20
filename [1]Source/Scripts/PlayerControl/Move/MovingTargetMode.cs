using System;
using UnityEngine;

namespace Scripts.PlayerControl.Move
{
    [CreateAssetMenu(fileName = "Move Target Mode", menuName = "MyCustom/Player/Move Target Mode")]
    public class MovingTargetMode : MovingMode
    {
        [Header("TargetMode")] [SerializeField]
        private float startDistance = 2;

        [SerializeField] private Vector2 minMaxDistance;
        [Range(0, 1)] public float smoothZoom = 0.1f;
        private Vector3 _prePosition;

        public MovingTargetMode()
        {
            bodyScale = new Vector3(1f, 1f, 1f);
            UseGravity = false;
            ColliderType = null;
            ;
            CamPosition = new Vector3(0f, 0f, 0f);
        }

        public override void Invoke()
        {
            CameraMove();
        }

        private void CameraMove()
        {
            var delta = Input.mouseScrollDelta.y;
            if (Math.Abs(delta) > 0)
            {
                var pos = _prePosition;
                pos.z = Mathf.Clamp(pos.z + delta, minMaxDistance.x, minMaxDistance.y);
                _prePosition = pos;
            }

            Cam.transform.localPosition = Vector3.Lerp(Cam.transform.localPosition,
                _prePosition,
                Time.fixedDeltaTime / smoothZoom);
        }

        public void MoveTo(Vector3 position)
        {
            Body.position = position;

            Cam.transform.localPosition = Vector3.back * (startDistance / 2);
            _prePosition = Vector3.back * startDistance;
        }
    }
}