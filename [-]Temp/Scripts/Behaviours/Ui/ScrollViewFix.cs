using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.System.Ui
{
    [global::System.Serializable]
    public struct ToggleAndHeight
    {
        public Toggle Toggle;
        public float Height;
    }

    /// <summary>
    /// При нажатии на тоггл меняет размер контейнера.
    /// </summary>
    public class ScrollViewFix : MonoBehaviour
    {
        [SerializeField] private RectTransform container;
        [Space] [SerializeField] private List<ToggleAndHeight> toggleAndHeight;

        private void Awake()
        {
            foreach (var item in toggleAndHeight)
            {
                item.Toggle.onValueChanged.AddListener((x) =>
                    container.sizeDelta = new Vector2(container.sizeDelta.x, item.Height));
                container.position = new Vector3();
            }

            container.position = new Vector3();
        }
    }
}