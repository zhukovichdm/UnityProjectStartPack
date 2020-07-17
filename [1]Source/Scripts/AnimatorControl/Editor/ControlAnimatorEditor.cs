using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Behaviours;
using Scripts.System;
using Scripts.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(ControlAnimator))]
public class ControlAnimatorEditor : Editor
{
    private ControlAnimator ControlAnimator => (ControlAnimator) target;
    private List<SystemAnimator> SystemAnimators => ControlAnimator.systemAnimators;

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(ControlAnimator, "Changed Control Animator");
        EditorGUI.BeginChangeCheck();

        Box.PutInVerticalBox(true, true, () =>
        {
            DrawOther();
            DrawList();
        });

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(ControlAnimator);
    }

    private void DrawOther()
    {
        Box.PutInHorizontalBox(true, false, () =>
        {
            ControlAnimator.useInputButton = EditorGUILayout.Toggle("useInputButton", ControlAnimator.useInputButton);
            if (ControlAnimator.useInputButton)
            {
                var buttonList = Enum.GetNames(typeof(PointerEventData.InputButton)).ToArray();
                ControlAnimator.inputButton =
                    (PointerEventData.InputButton) EditorGUILayout.Popup((int) ControlAnimator.inputButton, buttonList);
            }
        });


        Box.PutInHorizontalBox(true, false, () =>
        {
            var modeList = Enum.GetNames(typeof(AnimationModes)).ToArray();
            EditorGUILayout.LabelField($"Reverse mode", EditorStyles.label, GUILayout.MaxWidth(100));
            var popup = (AnimationModes) EditorGUILayout.Popup((int) ControlAnimator.mode, modeList);
            if (popup != ControlAnimator.mode)
            {
                ControlAnimator.mode = popup;
                ControlAnimator.Start();
            }
        });

        EditorGUILayout.Space();

        if (GUILayout.Button("UpdateAllParameters"))
            UpdateParameters();
        EditorGUILayout.Space();
    }

    private void DrawList()
    {
        for (var i = 0; i < SystemAnimators.Count; i++)
        {
            Box.PutInVerticalBox(true, true, (() =>
            {
                // TODO: Убрать эту проверку. Исправить баг при удалении элемента списка.
                if (TopElements(i))
                    return;
                MiddleElements(i);
            }));
            EditorGUILayout.Space();
        }

        BottomElements();
    }

    private bool TopElements(int i)
    {
        bool flag = false;
        var systemAnimator = SystemAnimators[i];

        Box.PutInVerticalBox(true, false, () =>
        {
            Box.PutInHorizontalBox(true, false, () =>
            {
                EditorGUILayout.LabelField($"Item {i}", EditorStyles.boldLabel, GUILayout.MaxWidth(80));
                systemAnimator.animator =
                    (Animator) EditorGUILayout.ObjectField(systemAnimator.animator, typeof(Animator), true);

                if (GUILayout.Button("Remove", GUILayout.MaxWidth(70)))
                {
                    SystemAnimators.RemoveAt(i);
                    flag = true;
                }
            });

            Box.PutInHorizontalBox(true, false, () =>
            {
                if (systemAnimator.animator && GUILayout.Button("UpdateParameters"))
                    SystemAnimators[i].UpdateParameters();

                if (GUILayout.Button("Up", GUILayout.MaxWidth(70)))
                {
                    if (i - 1 >= 0)
                    {
                        var buff = SystemAnimators[i - 1];
                        SystemAnimators[i - 1] = SystemAnimators[i];
                        SystemAnimators[i] = buff;
                    }
                }

                if (GUILayout.Button("Down", GUILayout.MaxWidth(70)))
                {
                    if (i + 1 < SystemAnimators.Count)
                    {
                        var buff = SystemAnimators[i + 1];
                        SystemAnimators[i + 1] = SystemAnimators[i];
                        SystemAnimators[i] = buff;
                    }
                }
            });
        });

        return flag;
    }

    private void MiddleElements(int i)
    {
        var systemAnimator = SystemAnimators[i];
        if (systemAnimator.animator == null || systemAnimator.allAnimatorParameters.Count == 0) return;

        systemAnimator.selectedParameter = EditorGUILayout.Popup("Parameters", systemAnimator.selectedParameter,
            systemAnimator.allAnimatorParameters.ToArray());

        Box.PutInHorizontalBox(true, false, () =>
        {
            EditorGUILayout.LabelField("Direction", GUILayout.MinWidth(30), GUILayout.MaxWidth(100));

            Box.PutInHorizontalBox(true, false, () =>
            {
                if (GUILayout.Toggle(systemAnimator.Sign == AnimationDirection.ToBeginning, "To the beginning",
                    new GUIStyle(GUI.skin.button)))
                    systemAnimator.SetSign(AnimationDirection.ToBeginning);
                if (GUILayout.Toggle(systemAnimator.Sign == AnimationDirection.ToEnd, "To the end",
                    new GUIStyle(GUI.skin.button)))
                    systemAnimator.SetSign(AnimationDirection.ToEnd);
            });
        });

        systemAnimator.frameRate = EditorGUILayout.FloatField("FrameRate", systemAnimator.frameRate);

        if (systemAnimator.allAnimatorParameters.Count > systemAnimator.selectedParameter)
            systemAnimator.SetState(EditorGUILayout.Slider("State", systemAnimator.State, 0, 1));


        systemAnimator.playFromStart = EditorGUILayout.Toggle("PlayFromStart", systemAnimator.playFromStart);

        Box.PutInHorizontalBox(true, false, () =>
        {
            if (GUILayout.Button("Reverse"))
            {
                if (Application.isPlaying)
                    systemAnimator.ReverseAndPlayback();
                else
                {
                    systemAnimator.Reverse();
                    if (systemAnimator.Sign == AnimationDirection.ToEnd) systemAnimator.SetState(1);
                    else if (systemAnimator.Sign == AnimationDirection.ToBeginning) systemAnimator.SetState(0);
                }
            }

            if (GUILayout.Button(systemAnimator.isPlayback ? "Stop" : "Play current"))
            {
                if (systemAnimator.isPlayback)
                    systemAnimator.isPlayback = false;
                else if (Application.isPlaying)
                {
                    if (systemAnimator.Sign == AnimationDirection.ToEnd) systemAnimator.SetState(0);
                    else if (systemAnimator.Sign == AnimationDirection.ToBeginning) systemAnimator.SetState(1);
                    systemAnimator.Playback();
                }
            }

            if (GUILayout.Button("Reset"))
                systemAnimator.Reset();
        });
    }

    private void BottomElements()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("Add"))
        {
            var systemAnimator = new SystemAnimator();
            systemAnimator.Initialize(ControlAnimator);
            SystemAnimators.Add(systemAnimator);
        }
    }

    private void UpdateParameters()
    {
        foreach (var systemAnimator in SystemAnimators)
            systemAnimator.UpdateParameters();
    }
}