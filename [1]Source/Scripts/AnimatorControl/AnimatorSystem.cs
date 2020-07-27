using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Scripts.System
{
    public static class AnimationControlExtensions
    {
        public static AnimationDirection Reverse(this AnimationDirection animDir) =>
            (AnimationDirection) ((int) animDir * -1);

        public static int Value(this AnimationDirection animDir) => (int) animDir;
    }

    public enum AnimationDirection
    {
        ToEnd = 1,
        ToBeginning = -1
    }

    [Serializable]
    public class AnimatorSystem
    {
        public MyAction actionPlayback_End = new MyAction();

        public MonoBehaviour mono;
        public Animator animator;
        public List<string> allAnimatorParameters = new List<string>();
        public float frameRate = 1;
        public int selectedParameter = 0;
        public bool playFromStart;
        public bool isPlayback;
        private Coroutine _coroutine;

        // INFO:  [field: SerializeField] требуется что бы значение не сбрасывалось редактором в ноль.
        [field: SerializeField] public float State { get; private set; } // От 0 до 1
        [field: SerializeField] public AnimationDirection Sign { get; private set; } // 1 либо -1

        public void Initialize(MonoBehaviour monoBehaviour)
        {
            mono = monoBehaviour;
        }

        public void OnEnable()
        {
            if (playFromStart && animator)
                Playback();
        }

        public void Playback()
        {
            if (isPlayback == false && allAnimatorParameters.Count > selectedParameter &&
                animator.gameObject.activeInHierarchy)
                _coroutine = mono.StartCoroutine(Playback());

            IEnumerator Playback()
            {
                isPlayback = true;
                const float tolerance = 0f;
                while (isPlayback && (
                           Math.Abs(State) > tolerance && Sign == AnimationDirection.ToBeginning ||
                           Math.Abs(State - 1f) > tolerance && Sign == AnimationDirection.ToEnd))
                {
                    var step = State + Time.deltaTime * (int) Sign * frameRate;
                    State = Mathf.Clamp(step, 0, 1f);
                    animator.SetFloat(allAnimatorParameters[selectedParameter], State);
                    yield return null;
                }

                animator.SetFloat(allAnimatorParameters[selectedParameter], State);

                isPlayback = false;
                actionPlayback_End.Publish();
            }
        }

        public void ReverseAndPlayback()
        {
            Reverse();
            Playback();
        }

        public void Reverse()
        {
            Sign = Sign.Reverse();
        }

        public void SetSignAndPlayback(AnimationDirection direction)
        {
            SetSign(direction);
            Playback();
        }

        public void SetSign(AnimationDirection direction)
        {
            Sign = direction;
        }

        public void SetState(float value)
        {
            animator.SetFloat(allAnimatorParameters[selectedParameter], value);
            State = value;
        }

        public void Reset()
        {
            if (_coroutine != null) mono.StopCoroutine(_coroutine);
            isPlayback = false;
            State = 0;
            animator.SetFloat(allAnimatorParameters[selectedParameter], State);
            Sign = AnimationDirection.ToBeginning;
        }

        public void UpdateParameters()
        {
            allAnimatorParameters.Clear();
            if (animator == false) return;
            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) return;
            foreach (var parameter in controller.parameters)
                allAnimatorParameters.Add(parameter.name);
        }
    }
}