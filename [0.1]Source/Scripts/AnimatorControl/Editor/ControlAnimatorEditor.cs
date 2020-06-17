using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Scripts.Behaviours;
using Scripts.System;
using Scripts.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(ControlAnimator))]
public class ControlAnimatorEditor : Editor
{
    private ControlAnimator _controlAnimator;
    private List<SystemAnimator> _systemAnimators;

    private void OnEnable()
    {
        _controlAnimator = (ControlAnimator) target;
        _systemAnimators = _controlAnimator.systemAnimators;
        UpdateParameters();
    }

    public override void OnInspectorGUI()
    {
        Box.PutInVerticalBox(true, true, () =>
        {
            DrawOther();
            DrawList();
        });
        EditorUtility.SetDirty(_controlAnimator);
    }

    private void DrawOther()
    {
        var buttonList = Enum.GetNames(typeof(PointerEventData.InputButton)).ToArray();

        Box.PutInHorizontalBox(true, false, () =>
        {
            _controlAnimator.useInputButton = EditorGUILayout.Toggle("useInputButton", _controlAnimator.useInputButton);

            if (_controlAnimator.useInputButton)
                _controlAnimator.inputButton =
                    (PointerEventData.InputButton) EditorGUILayout.Popup((int) _controlAnimator.inputButton,
                        buttonList);
        });

        EditorGUILayout.Space();

        if (GUILayout.Button("UpdateAllParameters"))
            UpdateParameters();
        EditorGUILayout.Space();
    }

    private void DrawList()
    {
//        int i = 0;
//        while (i < _systemAnimators.Count)
//        {
//            i++;

        for (var i = 0; i < _systemAnimators.Count; i++)
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
        var systemAnimator = _systemAnimators[i];

        Box.PutInVerticalBox(true, true, () =>
        {
            Box.PutInHorizontalBox(true, false, () =>
            {
                EditorGUILayout.LabelField($"Item {i}", EditorStyles.boldLabel, GUILayout.MaxWidth(80));
                systemAnimator.animator =
                    (Animator) EditorGUILayout.ObjectField(systemAnimator.animator, typeof(Animator), true);
                if (GUILayout.Button("Remove", GUILayout.MaxWidth(70), GUILayout.Height(15)))
                {
                    _systemAnimators.RemoveAt(i);
                    flag = true;
                }
            });

            if (systemAnimator.animator && GUILayout.Button("UpdateParameters"))
                _systemAnimators[i].UpdateParameters();
        });

        return flag;
    }

    private void MiddleElements(int i)
    {
        var systemAnimator = _systemAnimators[i];
        if (systemAnimator.animator == null || systemAnimator.allAnimatorParameters.Count == 0) return;

        systemAnimator.selectedParameter = EditorGUILayout.Popup("Parameters", systemAnimator.selectedParameter,
            systemAnimator.allAnimatorParameters.ToArray());

        Box.PutInHorizontalBox(true, false, () =>
        {
            EditorGUILayout.LabelField("Direction", GUILayout.MinWidth(30), GUILayout.MaxWidth(100));

            Box.PutInHorizontalBox(true, false, () =>
            {
                if (GUILayout.Toggle(systemAnimator.Sign == AnimationDirection.ToEnd, "To the end",
                    new GUIStyle(GUI.skin.button)))
                    systemAnimator.SetSign(AnimationDirection.ToEnd);
                if (GUILayout.Toggle(systemAnimator.Sign == AnimationDirection.ToBeginning, "To the beginning",
                    new GUIStyle(GUI.skin.button)))
                    systemAnimator.SetSign(AnimationDirection.ToBeginning);
            });
        });

        systemAnimator.frameRate = EditorGUILayout.FloatField("FrameRate", systemAnimator.frameRate);

        if (systemAnimator.allAnimatorParameters.Count > systemAnimator.selectedParameter)
            systemAnimator.SetState(EditorGUILayout.Slider("State", systemAnimator.State, 0, 1));


        systemAnimator.playFromStart = EditorGUILayout.Toggle("PlayFromStart", systemAnimator.playFromStart);

        Box.PutInHorizontalBox(true, false, () =>
        {
            if (GUILayout.Button("Reverse"))
                systemAnimator.Reverse();

            if (GUILayout.Button(systemAnimator.isPlayback ? "Stop" : "Play current"))
            {
                if (systemAnimator.isPlayback)
                    systemAnimator.isPlayback = false;
                else
                    systemAnimator.Playback();
            }

            if (GUILayout.Button("Break"))
                systemAnimator.Break();
        });
    }

    private void BottomElements()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("Add"))
        {
            var systemAnimator = new SystemAnimator();
            systemAnimator.Initialize(_controlAnimator);
            _systemAnimators.Add(systemAnimator);
        }
    }

    private void UpdateParameters()
    {
        foreach (var systemAnimator in _systemAnimators)
            systemAnimator.UpdateParameters();
    }
}