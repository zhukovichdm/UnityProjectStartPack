using UnityEngine;

namespace Scripts.PlayerControl.Rotate
{
    [CreateAssetMenu(fileName = "Rotate Default Mode", menuName = "MyCustom/Player/Rotate Default Mode")]
    public class RotationDefaultMode : RotationMode
    {
        public bool rotateTheCameraSeparately = true;
        private float _time;
        private Vector2 _temp;
        private Rigidbody _rb;

        public override void Setup(Transform body, Transform cam)
        {
            base.Setup(body, cam);
            if (Body.TryGetComponent(out _rb) == false)
                _rb = body.gameObject.AddComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void Invoke()
        {
            if (Input.GetKey(key))
                GetDirection();
            Limitation();
            Rotation();
        }

        private void GetDirection()
        {
            direction.x += Input.GetAxis("Mouse Y") * (invertY ? 1 : -1) * sens % 360;
            direction.y -= Input.GetAxis("Mouse X") * (invertX ? 1 : -1) * sens % 360;
        }

        private void Limitation()
        {
            if (useLimitationX) direction.x = Mathf.Clamp(direction.x, limitationX.x, limitationX.y);
            if (useLimitationY) direction.y = Mathf.Clamp(direction.y, limitationY.x, limitationY.y);
        }

        private void Rotation()
        {
            if (rotateTheCameraSeparately)
            {
                _rb.MoveRotation(Quaternion.Slerp(Body.rotation, Quaternion.Euler(direction * Vector3.up),
                    Time.fixedDeltaTime / smooth));
                Cam.localRotation = Quaternion.Slerp(Cam.localRotation, Quaternion.Euler(direction * Vector3.right),
                    Time.fixedDeltaTime / smooth);
            }
            else
            {
                _rb.MoveRotation(Quaternion.Slerp(Body.rotation, Quaternion.Euler(direction),
                    Time.fixedDeltaTime / smooth));
            }
        }
    }
}