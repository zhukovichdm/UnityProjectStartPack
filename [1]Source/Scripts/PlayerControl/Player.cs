using System.Collections.Generic;
using Scripts.PlayerControl.Move;
using Scripts.PlayerControl.Rotate;
using UnityEngine;

// NOTE: https://ru.stackoverflow.com/questions/936026/%D0%9F%D1%80%D0%B0%D0%B2%D0%B8%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F-%D1%80%D0%B5%D0%B0%D0%BB%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8F-%D0%BF%D0%B5%D1%80%D0%B5%D0%B4%D0%B2%D0%B8%D0%B6%D0%B5%D0%BD%D0%B8%D1%8F-%D0%BF%D0%B5%D1%80%D1%81%D0%BE%D0%BD%D0%B0%D0%B6%D0%B0

namespace Scripts.PlayerControl
{
    [global::System.Serializable]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform cam;
        [SerializeField] private RotationDefaultMode rotationMode;
        public List<MovingMode> selectableMovingModes = new List<MovingMode>();
        [HideInInspector] public int selectedMovingMode;

        public MovingMode MovingMode => selectableMovingModes.Count > selectedMovingMode && selectedMovingMode != -1
            ? selectableMovingModes[selectedMovingMode]
            : null;

        private void Awake()
        {
            Setup();
            MovingMode.SetCollider();
        }

        private void OnValidate() => Setup();

        private void Update()
        {
            MovingMode.Invoke();
            rotationMode.Invoke();
        }

        public void Setup()
        {
            if (selectableMovingModes.Count < selectedMovingMode || selectedMovingMode == -1) selectedMovingMode = 0;
            if (MovingMode) MovingMode.Setup(transform, cam);
            if (rotationMode) rotationMode.Setup(transform, cam);
        }
    }
}