using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scripts.PlayerControl.Editor
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        private Player _player;

        private void OnEnable()
        {
            _player = (Player) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var modesList = _player.selectableMovingModes.Select(x => x?.name).ToArray();
            _player.selectedMovingMode = EditorGUILayout.Popup(_player.selectedMovingMode, modesList);
            _player.Setup();
            if (Application.isPlaying)
                _player.MovingMode.SetCollider();

            if (_player.MovingMode == null)
                EditorGUILayout.HelpBox("Не указан ни один из режимов перемещения", MessageType.Warning);

            EditorUtility.SetDirty(_player);
        }
    }
}