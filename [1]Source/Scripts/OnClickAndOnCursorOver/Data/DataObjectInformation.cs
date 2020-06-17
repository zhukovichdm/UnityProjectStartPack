using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "New ObjectInformation", menuName = "Object Information", order = 51)]
    public class DataObjectInformation : ScriptableObject
    {
        public new string name;
        [TextArea(3, 30)]
        public string shortDescription;
        [TextArea(3, 30)]
        public string description;
    }
}