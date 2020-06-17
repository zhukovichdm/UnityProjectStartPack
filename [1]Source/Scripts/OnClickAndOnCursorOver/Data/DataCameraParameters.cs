using Scripts.Component;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "New CameraParameters", menuName = "Camera Parameters", order = 51)]
    public class DataCameraParameters : ScriptableObject
    {
        public DataScrollParameter dataScrollParameter;
        public DataLimitationParameter dataLimitationParameter;
    }
}