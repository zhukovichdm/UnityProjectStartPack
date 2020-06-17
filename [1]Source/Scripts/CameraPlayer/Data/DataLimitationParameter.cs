using UnityEngine;

namespace Scripts.Component
{
    [global::System.Serializable]
    public class DataLimitationParameter
    {
        [SerializeField] private bool xRotationLimitation;
        [SerializeField] private bool yRotationLimitation;
        [SerializeField] private float angleCameraMinX = -90;
        [SerializeField] private float angleCameraMaxX = 90;
        [SerializeField] private float angleCameraMinY = -360;
        [SerializeField] private float angleCameraMaxY = 360;

        public Vector2 Limit(Vector2 value)
        {
            if (xRotationLimitation)
            {
                value.x %= 360;
                if (value.x <= angleCameraMinX)
                    value.x = angleCameraMinX;
                if (value.x >= angleCameraMaxX)
                    value.x = angleCameraMaxX;
            }

            if (yRotationLimitation)
            {
                value.y %= 360;
                if (value.y <= angleCameraMinY)
                    value.y = angleCameraMinY;
                if (value.y >= angleCameraMaxY)
                    value.y = angleCameraMaxY;
            }

            return value;
        }
    }
}