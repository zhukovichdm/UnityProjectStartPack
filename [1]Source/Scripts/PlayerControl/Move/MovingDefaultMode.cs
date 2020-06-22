using UnityEngine;

namespace Scripts.PlayerControl.Move
{
    [CreateAssetMenu(fileName = "Move Default Mode", menuName = "MyCustom/Player/Move Default Mode")]
    public class MovingDefaultMode : MovingMode
    {
        private float _time;
        public bool useGravity = true;
        public float speed = 1;

        public AnimationCurve smooth =
            new AnimationCurve(new Keyframe(0f, 0f, 0f, 0.5f), new Keyframe(1f, 1f, 0.5f, 0f));


        public override void Invoke()
        {
            var dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (dir == Vector3.zero)
            {
                _time = 0;
                return;
            }

            _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0, 1);
            Body.Translate(Time.fixedDeltaTime * smooth.Evaluate(_time) * speed * dir);
        }
    }
}