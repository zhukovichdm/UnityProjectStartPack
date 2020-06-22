using System;
using UnityEngine;

namespace Scripts.PlayerControl.Move
{
    [CreateAssetMenu(fileName = "Move Physic Mode", menuName = "MyCustom/Player/Move Physic Mode")]
    public class MovingPhysicMode : MovingMode
    {
        public KeyCode jump = KeyCode.Space;
        public float jumpForce = 5f;
        [Range(-2f, 2f)] public float height = 1.8f;

        public MovingPhysicMode()
        {
            bodyScale = new Vector3(0.7f, 1f, 0.7f);
            UseGravity = true;
            ColliderType = typeof(CapsuleCollider);
            CamPosition = new Vector3(0, height / 2f, 0);
        }

        public override void Invoke()
        {
            if (IsGrounding == false) return;
            Jump();
            Moving();
        }

        public override void Setup(Transform body, Transform cam)
        {
            base.Setup(body, cam);
            CamPosition = new Vector3(0, height / 2f, 0);
        }

        private bool IsGrounding
        {
            get
            {
                if (Collider == null) return false;
                var bounds = Collider.bounds;
                var bottomCenterPoint = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
                return Physics.CheckCapsule(bounds.center, bottomCenterPoint, bounds.size.x / 2 * 0.9f, groundLayer);
            }
        }

        private void Jump()
        {
            if (Input.GetKeyDown(jump))
                Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void Moving()
        {
            var dir = new Vector3();
            var ws = Input.GetAxis("Vertical");
            var ad = Input.GetAxis("Horizontal");
            if (Math.Abs(ws) > 0) dir += Body.forward * ws;
            if (Math.Abs(ad) > 0) dir += Body.right * ad;
            if (Input.GetKey(speedBoost)) dir *= speedBoostFactor;
            Rb.AddForce(dir * speed, ForceMode.Impulse);
        }
    }
}