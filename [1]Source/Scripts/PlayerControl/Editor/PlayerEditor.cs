using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scripts.PlayerControl.Editor
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        private Player Player => (Player) target;

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(Player, "Changed Player");
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();
            var modesList = Player.selectableMovingModes.Select(x => x?.name).ToArray();
            Player.selectedMovingMode = EditorGUILayout.Popup(Player.selectedMovingMode, modesList);
            Player.Setup();

            if (Player.MovingMode == null)
                EditorGUILayout.HelpBox("Не указан ни один из режимов перемещения", MessageType.Warning);
            else if (Application.isPlaying)
                Player.MovingMode.SetCollider();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(Player);
        }
    }
}