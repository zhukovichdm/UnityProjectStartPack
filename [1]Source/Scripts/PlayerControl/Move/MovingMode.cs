using System;
using UnityEngine;

namespace Scripts.PlayerControl.Move
{
    public abstract class MovingMode : ScriptableObject
    {
        public Mesh mesh;
        public Vector3 bodyScale;
        public LayerMask groundLayer = 1;
        public float drag = 2;
        public float speed = 0.2f;
        public float speedBoostFactor = 2f;
        public KeyCode speedBoost = KeyCode.LeftShift;
        protected Transform Body;
        protected Transform Cam;
        protected Rigidbody Rb;
        protected Collider Collider;
        protected bool UseGravity;
        protected Type ColliderType;
        protected Vector3 CamPosition;

//        private void OnValidate() => Setup(Body, Cam);
        public abstract void Invoke();

        public virtual void Setup(Transform body, Transform cam)
        {
            Body = body;
            Cam = cam;
            if (Body)
            {
                Body.localScale = bodyScale;
                body.GetComponent<MeshFilter>().mesh = mesh;

                if (Body.TryGetComponent(out Rb) == false)
                    Rb = body.gameObject.AddComponent<Rigidbody>();
                Rb.constraints = RigidbodyConstraints.FreezeRotation;
                Rb.drag = drag;
                Rb.useGravity = UseGravity;
            }

            if (Cam) Cam.localPosition = CamPosition;
        }

        public void SetCollider()
        {
            if (Body.TryGetComponent(out Collider))
            {
                if (Collider && Collider.GetType() != ColliderType)
                    Destroy(Body.gameObject.GetComponent<Collider>());
            }

            if (Body.TryGetComponent(out Collider) == false)
                if (ColliderType == typeof(SphereCollider))
                {
                    Collider = Body.gameObject.AddComponent<SphereCollider>();
                    ((SphereCollider) Collider).radius = 0.5f;
                }
                else if (ColliderType == typeof(CapsuleCollider))
                {
                    Collider = Body.gameObject.AddComponent<CapsuleCollider>();
                }
        }
    }
}