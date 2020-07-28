using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Behaviours;
using Scripts.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(AnimatorControl))]
public class AnimatorControlEditor : Editor
{
    private AnimatorControl AnimatorControl => (AnimatorControl) target;
    private List<AnimatorSystem> AnimatorSystems => AnimatorControl.animatorSystems;
    private List<AnimatorStruct> AnimatorStructs => AnimatorControl.animatorStructs;

    private readonly string[] _buttonList = Enum.GetNames(typeof(PointerEventData.InputButton)).ToArray();
    private readonly string[] _modeList = Enum.GetNames(typeof(AnimationModes)).ToArray();

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(AnimatorControl, "Changed Control Animator");
        EditorGUI.BeginChangeCheck();

        DrawAnimatorControl(AnimatorControl);
        DrawAnimatorStruct(AnimatorStructs);
        if (GUILayout.Button("Add"))
            AddElement(AnimatorStructs);


        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(AnimatorControl);
    }

    private void DrawAnimatorControl(AnimatorControl control)
    {
        EditorGroup.Horizontal(() => control.ignoreUi = EditorGUILayout.Toggle("ignoreUi", control.ignoreUi));
        EditorGroup.Horizontal(() =>
        {
            control.useInputButton = EditorGUILayout.Toggle("useInputButton", control.useInputButton);
            if (control.useInputButton == false) return;
            control.inputButton =
                (PointerEventData.InputButton) EditorGUILayout.Popup((int) control.inputButton, _buttonList);
        });
        EditorGroup.Horizontal(() =>
        {
            EditorGUILayout.LabelField($"Reverse mode", EditorStyles.label, GUILayout.MaxWidth(100));
            var popup = (AnimationModes) EditorGUILayout.Popup((int) control.mode, _modeList);
            if (popup == control.mode) return;
            control.mode = popup;
            control.ModeChanged();
        });

        if (GUILayout.Button("UpdateAllParameters"))
        {
            foreach (var systemAnimator in control.animatorSystems)
                systemAnimator.UpdateParameters();
        }
    }

    private void DrawAnimatorStruct(List<AnimatorStruct> animatorStructs)
    {
        for (var i = 0; i < animatorStructs.Count; i++)
        {
            var animatorStruct = animatorStructs[i];
            EditorGroup.VerticalBox(() =>
            {
                DrawButtons(animatorStructs, i);
                DrawAnimatorSystem(animatorStruct.animatorSystem);
                DrawAnimatorStruct(animatorStruct.animatorStructs);
            });
        }
    }

    private void DrawButtons(List<AnimatorStruct> animatorStructs, int i)
    {
        EditorGroup.Horizontal(() =>
        {
            if (GUILayout.Button("Add Child", EditorStyles.miniButtonLeft))
                AddElement(animatorStructs[i].animatorStructs);

            if (GUILayout.Button(new GUIContent("\u2191", "Move up"), EditorStyles.miniButtonMid,
                GUILayout.MaxWidth(70)))
                Swap(i, i - 1);

            if (GUILayout.Button(new GUIContent("\u2193", "Move down"), EditorStyles.miniButtonMid,
                GUILayout.MaxWidth(70)))
                Swap(i, i + 1);

            EditorGroup.SetColor(new Color(1, 0.7f, 0.7f), () =>
            {
                if (GUILayout.Button(new GUIContent("\u00d7", "Remove"), EditorStyles.miniButtonRight,
                    GUILayout.MaxWidth(45)))
                    animatorStructs.RemoveAt(i);
            });

            void Swap(int oldPos, int newPos)
            {
                if (newPos < 0 || newPos >= animatorStructs.Count) return;
                var buff = animatorStructs[newPos];
                animatorStructs[newPos] = animatorStructs[oldPos];
                animatorStructs[oldPos] = buff;
            }
        });
    }

    private void DrawAnimatorSystem(AnimatorSystem animSys)
    {
        EditorGroup.Horizontal(() =>
        {
            animSys.animator = (Animator) EditorGUILayout.ObjectField(animSys.animator, typeof(Animator), true);
            if (GUILayout.Button("UpdateParameters", EditorStyles.miniButtonRight))
                animSys.UpdateParameters();
        });

        var parameters = animSys.allAnimatorParameters.ToArray();
        animSys.selectedParameter = EditorGUILayout.Popup("Parameters", animSys.selectedParameter, parameters);

        EditorGroup.Horizontal(() =>
        {
            EditorGUILayout.LabelField("Direction", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
            var isToBeginning = animSys.Sign == AnimationDirection.ToBeginning;
            if (GUILayout.Toggle(isToBeginning, "<-", EditorStyles.miniButtonLeft))
                animSys.SetSign(AnimationDirection.ToBeginning);
            var isToEnd = animSys.Sign == AnimationDirection.ToEnd;
            if (GUILayout.Toggle(isToEnd, "->", EditorStyles.miniButtonRight))
                animSys.SetSign(AnimationDirection.ToEnd);
        });
        EditorGroup.Vertical(() =>
        {
            animSys.frameRate = EditorGUILayout.FloatField("FrameRate", animSys.frameRate);
            if (animSys.allAnimatorParameters.Count > animSys.selectedParameter)
            {
                var newState = EditorGUILayout.Slider("State", animSys.State, 0, 1);
                if (Math.Abs(newState - animSys.State) > 0)
                    animSys.SetState(newState);
            }

            animSys.playFromStart = EditorGUILayout.Toggle("PlayFromStart", animSys.playFromStart);
        });

        EditorGroup.Horizontal(() =>
        {
            if (GUILayout.Button("Reverse", EditorStyles.miniButtonLeft))
            {
                if (Application.isPlaying)
                    animSys.ReverseAndPlayback();
                else
                {
                    animSys.Reverse();
                    if (animSys.Sign == AnimationDirection.ToEnd) animSys.SetState(1);
                    else if (animSys.Sign == AnimationDirection.ToBeginning) animSys.SetState(0);
                }
            }

            if (GUILayout.Button(animSys.isPlayback ? "Stop" : "Play current", EditorStyles.miniButtonMid))
            {
                if (animSys.isPlayback)
                    animSys.isPlayback = false;
                else if (Application.isPlaying)
                {
                    if (animSys.Sign == AnimationDirection.ToEnd) animSys.SetState(0);
                    else if (animSys.Sign == AnimationDirection.ToBeginning) animSys.SetState(1);
                    animSys.Playback();
                }
            }

            if (GUILayout.Button("Reset", EditorStyles.miniButtonRight))
                animSys.Reset();
        });
    }

    private void AddElement(List<AnimatorStruct> animatorStructs)
    {
        var animatorStruct = new AnimatorStruct
        {
            animatorSystem = new AnimatorSystem(),
            animatorStructs = new List<AnimatorStruct>()
        };
        animatorStruct.animatorSystem.Initialize(AnimatorControl);
        animatorStructs.Add(animatorStruct);
    }
}