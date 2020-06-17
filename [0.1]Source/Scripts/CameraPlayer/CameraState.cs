using UnityEngine;

namespace Scripts.Modules.CameraPlayer
{
    public class CameraState
    {
        public float Yaw;
        public float Pitch;
        public float Roll;
        public float X;
        public float Y;
        public float Z;

        public void SetFromTransform(Transform t)
        {
            Pitch = t.eulerAngles.x;
            Yaw = t.eulerAngles.y;
            Roll = t.eulerAngles.z;
            X = t.position.x;
            Y = t.position.y;
            Z = t.position.z;
        }

        public void SetPositionFromTransform(Transform t)
        {
            X = t.position.x;
            Y = t.position.y;
            Z = t.position.z;
        }

        public void SetRotation(Vector3 rotation)
        {
            Pitch = rotation.x;
            Yaw = rotation.y;
            Roll = rotation.z;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(Pitch, Yaw, Roll) * translation;

            X += rotatedTranslation.x;
            Y += rotatedTranslation.y;
            Z += rotatedTranslation.z;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            Yaw = Mathf.Lerp(Yaw, target.Yaw, rotationLerpPct);
            Pitch = Mathf.Lerp(Pitch, target.Pitch, rotationLerpPct);
            Roll = Mathf.Lerp(Roll, target.Roll, rotationLerpPct);

            X = Mathf.Lerp(X, target.X, positionLerpPct);
            Y = Mathf.Lerp(Y, target.Y, positionLerpPct);
            Z = Mathf.Lerp(Z, target.Z, positionLerpPct);
        }

        public void LerpTowardsAngle(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            Yaw = Mathf.Lerp(Yaw, target.Yaw, rotationLerpPct);
            Pitch = Mathf.Lerp(Pitch, target.Pitch, rotationLerpPct);
            Roll = Mathf.Lerp(Roll, target.Roll, rotationLerpPct);
        }


        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(Pitch, Yaw, Roll);
            t.position = new Vector3(X, Y, Z);
        }

        public void UpdateTransformAngle(Transform t)
        {
            t.eulerAngles = new Vector3(Pitch, Yaw, Roll);
        }


        public void UpdateRotation(CameraState target)
        {
            Yaw = target.Yaw;
            Pitch = target.Pitch;
            Roll = target.Roll;

            X = target.X;
            Y = target.Y;
            Z = target.Z;

//            t.eulerAngles = new Vector3(Pitch, Yaw, Roll);
        }
    }
}