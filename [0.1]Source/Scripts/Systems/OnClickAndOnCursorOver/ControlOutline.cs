using Scripts.Component;
using Scripts.Component.Actions;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Behaviours
{
    public class ControlOutline : MonoBehaviour
    {
        [ColorUsage(true, true)]
        [SerializeField] private Color outlineColor;

        private ControlObjectInformation _lastControl;
        private GameObject _lastOutlineObject;
        private GameObject _lastSelectedObject;

        private void Awake()
        {
            GameActions.UpdateObjectInformation.Subscribe(value =>
            {
                _lastSelectedObject = value.Item2;
                if (_lastSelectedObject == _lastOutlineObject)
                    ApplyBaseColor(_lastControl);
            });
            GameActions.CursorOverObject.Subscribe(Outline);
            UserInput.InputEscapeAction.Subscribe(() => _lastSelectedObject = null);
        }

        private void Outline((bool enter, ControlObjectInformation data) valueTuple)
        {
            if (Testing.TestingMode) return;
            ApplyBaseColor(_lastControl);
            if (valueTuple.enter) ApplyOutlineColor(valueTuple.data);
        }

        private void ApplyOutlineColor(ControlObjectInformation control)
        {
            if (_lastSelectedObject == control.gameObject) return;

            foreach (var meshRenderer in control.meshRenderers)
            {
                // TODO: раньше можно было для каждого объекта указывать отдельный материал, сделать то же самое для цветов.
                meshRenderer.material.SetColor("_EmissionColor", outlineColor);
            }

            _lastControl = control;
            _lastOutlineObject = control.gameObject;
        }

        private void ApplyBaseColor(ControlObjectInformation control)
        {
            if (!_lastControl) return;
            foreach (var meshRenderer in control.meshRenderers)
            {
                meshRenderer.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}