namespace Scripts.Component
{
    [global::System.Serializable]
    public class DataScrollParameter
    {
        public bool scrollCollisionDetect;
        public float autoScrollSensitivity = 10f;
        public float scrollSensitivity = 5f;
        public float minDistance = -0.2f;
        public float maxDistance = -10f;
    }
}