using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Group1
{
    [CreateAssetMenu(fileName = "New ClickParameters", menuName = "Click Parameters", order = 51)]
    public class DataClickParameters : ScriptableObject
    {
        public PointerEventData.InputButton inputButton;
        public int clickCount = 2;
        public float maxDistanceToMainCamera = 100;
    }
}