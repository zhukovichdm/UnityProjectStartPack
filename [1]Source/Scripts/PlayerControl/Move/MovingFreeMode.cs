using System;
using UnityEngine;

namespace Scripts.PlayerControl.Move
{
    [CreateAssetMenu(fileName = "Move Free Mode", menuName = "MyCustom/Player/Move Free Mode")]
    public class MovingFreeMode : MovingMode
    {
        public KeyCode up = KeyCode.E;
        public KeyCode down = KeyCode.Q;

        public MovingFreeMode()
        {
            bodyScale = new Vector3(1f, 1f, 1f);
            UseGravity = false;
            ColliderType = typeof(SphereCollider);
            CamPosition = new Vector3(0f, 0f, 0f);
        }

        public override void Invoke()
        {
            Moving();
        }

        private void Moving()
        {
            var dir = new Vector3();
            var ws = Input.GetAxis("Vertical");
            var ad = Input.GetAxis("Horizontal");
            if (Math.Abs(ws) > 0) dir += Cam.forward * ws;
            if (Math.Abs(ad) > 0) dir += Cam.right * ad;
            if (Input.GetKey(up)) dir += Cam.up;
            if (Input.GetKey(down)) dir -= Cam.up;
            if (Input.GetKey(speedBoost)) dir *= speedBoostFactor;
            Rb.AddForce(dir * speed, ForceMode.Impulse);
        }
    }
}